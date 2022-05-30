using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Serilog;

namespace DeepReader.Storage.Faster.ActionTraces
{
    public sealed class ActionTraceStore
    {
        private readonly FasterKV<ulong, ActionTrace> _store;

        //private readonly ClientSession<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceContext, ActionTraceFunctions> _actionTraceReaderSession;
        //private readonly ClientSession<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceContext, ActionTraceFunctions> _actionTraceWriterSession;

        private readonly AsyncPool<ClientSession<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceContext, ActionTraceFunctions>> _sessionPool;

        private int _sessionCount;

        private readonly FasterStorageOptions _options;

        private readonly ITopicEventSender _eventSender;
        private MetricsCollector _metricsCollector;

        private static readonly SummaryConfiguration SummaryConfiguration = new SummaryConfiguration()
            { MaxAge = TimeSpan.FromSeconds(30) };

        private static readonly Summary WritingActionTraceDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_write_action_trace_duration", "Summary of time to store actionTraces to Faster", SummaryConfiguration);
        private static readonly Summary StoreLogMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_log_memory_size_bytes", "Summary of the faster actionTrace store log memory size in bytes", SummaryConfiguration);
        private static readonly Summary StoreReadCacheMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_read_cache_memory_size_bytes", "Summary of the faster actionTrace store read cache memory size in bytes", SummaryConfiguration);
        private static readonly Summary StoreEntryCountSummary =
           Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_read_cache_memory_size_bytes", "Summary of the faster actionTrace store entry count", SummaryConfiguration);
        private static readonly Summary StoreTakeLogCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_take_log_checkpoint_duration", "Summary of time to take a log checkpoint of faster actionTrace store", SummaryConfiguration);
        private static readonly Summary StoreTakeIndexCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_take_index_checkpoint_duration", "Summary of time to take a index checkpoint of faster actionTrace store", SummaryConfiguration);
        private static readonly Summary StoreFlushAndEvictLogDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_action_trace_store_log_flush_and_evict_duration", "Summary of time to flush and evict faster actionTrace store", SummaryConfiguration);
        private static readonly Summary ActionTraceReaderSessionReadDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_action_trace_get_by_id_duration", "Summary of time to try get actionTrace trace by id", SummaryConfiguration);

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
                PageSizeBits = 14, // (16K pages)
                MemorySizeBits = 33 // (8G memory for main log)
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
                try
                {
                    Log.Information("Recovering ActionTraceStore");
                    _store.Recover(1);
                    Log.Information("ActionTraceStore recovered");
                }
                catch (Exception e)
                {
                    Log.Error(e, "");
                    throw;
                }
            }

            //foreach (var recoverableSession in _store.RecoverableSessions)
            //{
            //    if (recoverableSession.Item2 == "ActionTraceWriterSession")
            //    {
            //        _actionTraceWriterSession = _store.For(new ActionTraceFunctions())
            //            .ResumeSession<ActionTraceFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
            //    }
            //    else if (recoverableSession.Item2 == "ActionTraceReaderSession")
            //    {
            //        _actionTraceReaderSession = _store.For(new ActionTraceFunctions())
            //            .ResumeSession<ActionTraceFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
            //    }
            //}

            //_actionTraceWriterSession ??=
            //    _store.For(new ActionTraceFunctions()).NewSession<ActionTraceFunctions>("ActionTraceWriterSession");
            //_actionTraceReaderSession ??=
            //    _store.For(new ActionTraceFunctions()).NewSession<ActionTraceFunctions>("ActionTraceReaderSession");

            var actionTraceEvictionObserver = new ActionTraceEvictionObserver();
            _store.Log.SubscribeEvictions(actionTraceEvictionObserver);

            if (options.UseReadCache)
                _store.ReadCache.SubscribeEvictions(actionTraceEvictionObserver);

            _sessionPool = new AsyncPool<ClientSession<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceContext, ActionTraceFunctions>>(
                logSettings.LogDevice.ThrottleLimit,
                () => _store.For(new ActionTraceFunctions()).NewSession<ActionTraceFunctions>("ActionTraceSession" + Interlocked.Increment(ref _sessionCount)));

            foreach (var recoverableSession in _store.RecoverableSessions)
            {
                _sessionPool.Return(_store.For(new ActionTraceFunctions())
                    .ResumeSession<ActionTraceFunctions>(recoverableSession.Item2, out CommitPoint commitPoint));
                _sessionCount++;
            }

            new Thread(CommitThread).Start();
        }

        private void CollectObservableMetrics(object? sender, EventArgs e)
        {
            StoreLogMemorySizeBytesSummary.Observe(_store.Log.MemorySizeBytes);
            if (_options.UseReadCache)
                StoreReadCacheMemorySizeBytesSummary.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountSummary.Observe(_store.EntryCount);
        }

        public async Task<Status> WriteActionTrace(ActionTrace actionTrace)
        {
            var actionTraceId = actionTrace.GlobalSequence;

            await _eventSender.SendAsync("ActionTraceAdded", actionTrace);

            using (WritingActionTraceDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var result = await session.UpsertAsync(ref actionTraceId, ref actionTrace);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                _sessionPool.Return(session);
                return result.Status;
            }
        }

        public async Task<(bool, ActionTrace)> TryGetActionTraceById(ulong globalSequence)
        {
            using (ActionTraceReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = (await session.ReadAsync(globalSequence)).Complete();
                _sessionPool.Return(session);
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

                    using (StoreTakeLogCheckpointDurationSummary.NewTimer())
                        _store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver, true).GetAwaiter().GetResult();

                    if (_options.FlushAfterCheckpoint)
                    {
                        using (StoreFlushAndEvictLogDurationSummary.NewTimer())
                            _store.Log.FlushAndEvict(true);
                    }

                    if (logCheckpointsTaken % _options.IndexCheckpointMultiplier == 0)
                    {
                        using (StoreTakeIndexCheckpointDurationSummary.NewTimer())
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
