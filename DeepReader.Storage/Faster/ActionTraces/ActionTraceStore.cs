using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Sentry;
using Serilog;

namespace DeepReader.Storage.Faster.ActionTraces
{
    public sealed class ActionTraceStore
    {
        private readonly FasterKV<ulong, ActionTrace> _store;

        private readonly ClientSession<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceContext, ActionTraceFunctions> _actionTraceReaderSession;
        private readonly ClientSession<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceContext, ActionTraceFunctions> _actionTraceWriterSession;

        private readonly FasterStorageOptions _options;

        private readonly ITopicEventSender _eventSender;
        private MetricsCollector _metricsCollector;

        private static readonly Histogram WritingActionTraceDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_write_action_trace_duration", "Histogram of time to store actionTraces to Faster");
        private static readonly Histogram StoreLogMemorySizeBytesHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_action_trace_store_log_memory_size_bytes", "Histogram of the faster actionTrace store log memory size in bytes");
        private static readonly Histogram StoreReadCacheMemorySizeBytesHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_action_trace_store_read_cache_memory_size_bytes", "Histogram of the faster actionTrace store read cache memory size in bytes");
        private static readonly Histogram StoreEntryCountHistogram =
           Metrics.CreateHistogram("deepreader_storage_faster_action_trace_store_read_cache_memory_size_bytes", "Histogram of the faster actionTrace store entry count");
        private static readonly Histogram StoreTakeLogCheckpointDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_action_trace_store_take_log_checkpoint_duration", "Histogram of time to take a log checkpoint of faster actionTrace store");
        private static readonly Histogram StoreTakeIndexCheckpointDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_action_trace_store_take_index_checkpoint_duration", "Histogram of time to take a index checkpoint of faster actionTrace store");
        private static readonly Histogram StoreFlushAndEvictLogDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_action_trace_store_log_flush_and_evict_duration", "Histogram of time to flush and evict faster actionTrace store");
        private static readonly Histogram ActionTraceReaderSessionReadDurationHistogram =
          Metrics.CreateHistogram("deepreader_storage_faster_action_trace_get_by_id_duration", "Histogram of time to try get actionTrace trace by id");

        public ActionTraceStore(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector)
        {
            _options = options;
            _eventSender = eventSender;
            _metricsCollector = metricsCollector;
            _metricsCollector.CollectMetricsHandler += CollectObservableMetrics;

            if (!_options.ActionTraceStoreDir.EndsWith("/"))
                options.ActionTraceStoreDir += "/";

            // Create files for storing data
            var log = Devices.CreateLogDevice(_options.ActionTraceStoreDir + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(_options.ActionTraceStoreDir + "hlog.obj.log");

            // Define settings for log
            var logSettings = new LogSettings
            {
                LogDevice = log,
                ObjectLogDevice = objlog,
                ReadCacheSettings = _options.UseReadCache ? new ReadCacheSettings() : null,
                // to calculate below:
                // 12 = 00001111 11111111 = 4095 = 4K
                // 34 = 00000011 11111111 11111111 11111111 11111111 = 17179869183 = 16G
                PageSizeBits = 14, // (4K pages)
                MemorySizeBits = 35 // (16G memory for main log)
            };

            // Define serializers; otherwise FASTER will use the slower DataContract
            // Needed only for class keys/values
            var serializerSettings = new SerializerSettings<ulong, ActionTrace>
            {
                valueSerializer = () => new ActionTraceValueSerializer()
            };

            var checkPointsDir = _options.ActionTraceStoreDir + "checkpoints";

            var checkpointManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(checkPointsDir), true);

            _store = new FasterKV<ulong, ActionTrace>(
                size: _options.MaxActionTracesCacheEntries, // Cache Lines for ActionTraces
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointManager = checkpointManager },
                serializerSettings: serializerSettings
            );

            if (Directory.Exists(checkPointsDir))
            {
                Log.Information("Recovering ActionTraceStore");
                _store.Recover(1);
                Log.Information("ActionTraceStore recovered");
            }

            foreach (var recoverableSession in _store.RecoverableSessions)
            {
                if (recoverableSession.Item2 == "ActionTraceWriterSession")
                {
                    _actionTraceWriterSession = _store.For(new ActionTraceFunctions())
                        .ResumeSession<ActionTraceFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
                }
                else if (recoverableSession.Item2 == "ActionTraceReaderSession")
                {
                    _actionTraceReaderSession = _store.For(new ActionTraceFunctions())
                        .ResumeSession<ActionTraceFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
                }
            }

            _actionTraceWriterSession ??=
                _store.For(new ActionTraceFunctions()).NewSession<ActionTraceFunctions>("ActionTraceWriterSession");
            _actionTraceReaderSession ??=
                _store.For(new ActionTraceFunctions()).NewSession<ActionTraceFunctions>("ActionTraceReaderSession");

            var actionTraceEvictionObserver = new ActionTraceEvictionObserver();
            _store.Log.SubscribeEvictions(actionTraceEvictionObserver);

            if (options.UseReadCache)
                _store.ReadCache.SubscribeEvictions(actionTraceEvictionObserver);

            new Thread(CommitThread).Start();
        }

        private void CollectObservableMetrics(object? sender, EventArgs e)
        {
            StoreLogMemorySizeBytesHistogram.Observe(_store.Log.MemorySizeBytes);
            if (_options.UseReadCache)
                StoreReadCacheMemorySizeBytesHistogram.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountHistogram.Observe(_store.EntryCount);
        }

        public async Task<Status> WriteActionTrace(ActionTrace actionTrace)
        {
            var actionTraceId = actionTrace.GlobalSequence;

            await _eventSender.SendAsync("ActionTraceAdded", actionTrace);

            using (WritingActionTraceDurationHistogram.NewTimer())
            {
                var result = await _actionTraceWriterSession.UpsertAsync(ref actionTraceId, ref actionTrace);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                return result.Status;
            }
        }

        public async Task<(bool, ActionTrace)> TryGetActionTraceById(ulong globalSequence)
        {
            using (ActionTraceReaderSessionReadDurationHistogram.NewTimer())
            {
                var (status, output) = (await _actionTraceReaderSession.ReadAsync(globalSequence)).Complete();
                return (status.Found, output.Value);
            }
        }

        private void CommitThread()
        {
            if (_options.LogCheckpointInterval is null or 0)
                return;

            int logCheckpointsTaken = 0;

            while (true)
            {
                try
                {
                    Thread.Sleep(_options.LogCheckpointInterval.Value);

                    using (StoreTakeLogCheckpointDurationHistogram.NewTimer())
                        _store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver, true).GetAwaiter().GetResult();

                    if (_options.FlushAfterCheckpoint)
                    {
                        using (StoreFlushAndEvictLogDurationHistogram.NewTimer())
                            _store.Log.FlushAndEvict(true);
                    }

                    if (logCheckpointsTaken % _options.IndexCheckpointMultiplier == 0)
                    {
                        using (StoreTakeIndexCheckpointDurationHistogram.NewTimer())
                            _store.TakeIndexCheckpointAsync().GetAwaiter().GetResult();
                    }

                    logCheckpointsTaken++;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "");
                }
            }
        }
    }
}
