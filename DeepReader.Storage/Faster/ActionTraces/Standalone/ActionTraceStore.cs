using DeepReader.Storage.Faster.ActionTraces.Base;
using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Serilog;

namespace DeepReader.Storage.Faster.ActionTraces.Standalone
{
    public class ActionTraceStore : ActionTraceStoreBase
    {
        protected readonly FasterKV<ulong, ActionTrace> _store;

        private readonly AsyncPool<ClientSession<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceContext, ActionTraceFunctions>> _sessionPool;

        public ActionTraceStore(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
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
                serializerSettings: serializerSettings,
                new FasterSequentialULongKeyComparer()
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


        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {
            StoreLogMemorySizeBytesSummary.Observe(_store.Log.MemorySizeBytes);
            if (_options.UseReadCache)
                StoreReadCacheMemorySizeBytesSummary.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountSummary.Observe(_store.EntryCount);
        }

        public override async Task<Status> WriteActionTrace(ActionTrace actionTrace)
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

        public override async Task<(bool, ActionTrace)> TryGetActionTraceById(ulong globalSequence)
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

        internal void CommitThread()
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
