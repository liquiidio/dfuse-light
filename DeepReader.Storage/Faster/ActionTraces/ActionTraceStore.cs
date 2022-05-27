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

        private static readonly Histogram WritingActionTraceDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_write_action_trace_duration", "Histogram of time to store actionTraces to Faster");
        private static readonly Histogram StoreLogMemorySizeBytesHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_action_trace_store_log_memory_size_bytes", "Histogram of the faster actionTrace store log memory size in bytes");
        private static readonly Histogram StoreReadCacheMemorySizeBytesHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_action_trace_store_read_cache_memory_size_bytes", "Histogram of the faster actionTrace store read cache memory size in bytes");
        private static readonly Histogram StoreEntryCountHistogram =
           Metrics.CreateHistogram("deepreader_storage_faster_action_trace_store_read_cache_memory_size_bytes", "Histogram of the faster actionTrace store entry count");
        private static readonly Histogram StoreTakeFullCheckpointDurationHistogram =
          Metrics.CreateHistogram("deepreader_storage_faster_action_trace_store_take_full_checkpoint_duration", "Histogram of time to take a full checkpoint of faster actionTrace store");
        private static readonly Histogram StoreFlushAndEvictLogDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_action_trace_store_log_flush_and_evict_duration", "Histogram of time to flush and evict faster actionTrace store");
        private static readonly Histogram ActionTraceReaderSessionReadDurationHistogram =
          Metrics.CreateHistogram("deepreader_storage_faster_action_trace_get_by_id_duration", "Histogram of time to try get actionTrace trace by id");

        public ActionTraceStore(FasterStorageOptions options, ITopicEventSender eventSender)
        {
            _options = options;
            _eventSender = eventSender;

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
                PageSizeBits = 12, // (4K pages)
                MemorySizeBits = 34 // (16G memory for main log)
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

            StoreLogMemorySizeBytesHistogram.Observe(_store.Log.MemorySizeBytes);
            if (options.UseReadCache)
                StoreReadCacheMemorySizeBytesHistogram.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountHistogram.Observe(_store.EntryCount);

            // TODO, for some reason I need to manually call the Init
            SentrySdk.Init("https://b4874920c4484212bcc323e9deead2e9@sentry.noodles.lol/2");

            new Thread(CommitThread).Start();
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
            if (_options.CheckpointInterval is null or 0)
                return;

            while (true)
            {
                try
                {
                    Thread.Sleep(_options.CheckpointInterval.Value);

                    // Take log-only checkpoint (quick - no index save)
                    //store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver).GetAwaiter().GetResult();

                    // Take index + log checkpoint (longer time)
                    using (StoreTakeFullCheckpointDurationHistogram.NewTimer())
                        _store.TakeFullCheckpointAsync(CheckpointType.FoldOver).GetAwaiter().GetResult();
                    using (StoreFlushAndEvictLogDurationHistogram.NewTimer())
                        _store.Log.FlushAndEvict(true);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "");
                }
            }
        }
    }
}
