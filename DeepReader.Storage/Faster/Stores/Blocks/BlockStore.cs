using DeepReader.Storage.Faster.StoreBase;
using DeepReader.Storage.Faster.StoreBase.Standalone;
using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Serilog;

namespace DeepReader.Storage.Faster.Stores.Blocks
{
    public class BlockStore : BlockStoreBase
    {
        protected readonly FasterKV<long, Block> _store;

        private readonly AsyncPool<ClientSession<long, Block, Input, Block, KeyValueContext,
            StandaloneFunctions<long, Block>>> _sessionPool;

        public BlockStore(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            if (!_options.BlockStoreDir.EndsWith("/"))
                _options.BlockStoreDir += "/";


            // Create files for storing data
            var log = Devices.CreateLogDevice(_options.BlockStoreDir + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(_options.BlockStoreDir + "hlog.obj.log");

            // Define settings for log
            var logSettings = new LogSettings
            {
                LogDevice = log,
                ObjectLogDevice = objlog,
                ReadCacheSettings = options.UseReadCache ? new ReadCacheSettings() : null,
                // to calculate below:
                // 12 = 00001111 11111111 = 4095 = 4K
                // 34 = 11111111 11111111 11111111 11111111 = 17179869183 = 16G
                PageSizeBits = 14, // (16K pages)
                MemorySizeBits = 32 // (4G memory for main log)
            };

            // Define serializers; otherwise FASTER will use the slower DataContract
            // Needed only for class keys/values
            var serializerSettings = new SerializerSettings<long, Block>
            {
                keySerializer = () => new KeySerializer<LongKey, long>(),
                valueSerializer = () => new ValueSerializer<Block>()
            };

            var checkPointsDir = _options.BlockStoreDir + "checkpoints";

            var checkpointManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(checkPointsDir), true);


            _store = new FasterKV<long, Block>(
                size: _options.MaxBlocksCacheEntries, // Cache Lines for Blocks
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointManager = checkpointManager },
                serializerSettings: serializerSettings,
                new FasterSequentialLongKeyComparer()
            );

            if (Directory.Exists(checkPointsDir))
            {
                try
                {
                    Log.Information("Recovering BlockStore");
                    _store.Recover(1);
                    Log.Information("BlockStore recovered");
                }
                catch (Exception e)
                {
                    Log.Error(e, "");
                    throw;
                }
            }

            StoreLogMemorySizeBytesSummary.Observe(_store.Log.MemorySizeBytes);
            if (options.UseReadCache)
                StoreReadCacheMemorySizeBytesSummary.Observe(_store.ReadCache.MemorySizeBytes);// must be optional
            StoreEntryCountSummary.Observe(_store.EntryCount);

            var blockEvictionObserver = new PooledObjectEvictionObserver<long, Block>();
            _store.Log.SubscribeEvictions(blockEvictionObserver);

            if (options.UseReadCache)
                _store.ReadCache.SubscribeEvictions(blockEvictionObserver);

            _sessionPool = new AsyncPool<ClientSession<long, Block, Input, Block, KeyValueContext, StandaloneFunctions<long, Block>>>(
                logSettings.LogDevice.ThrottleLimit,
                () => _store.For(new StandaloneFunctions<long, Block>()).NewSession<StandaloneFunctions<long, Block>>("BlockSession" + Interlocked.Increment(ref _sessionCount)));

            foreach (var recoverableSession in _store.RecoverableSessions)
            {
                _sessionPool.Return(_store.For(new StandaloneFunctions<long, Block>())
                    .ResumeSession<StandaloneFunctions<long, Block>>(recoverableSession.Item2, out CommitPoint commitPoint));
                _sessionCount++;
            }

            new Thread(CommitThread).Start();
        }

        public long BlocksIndexed => _store.EntryCount;

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {
            StoreLogMemorySizeBytesSummary.Observe(_store.Log.MemorySizeBytes);
            if (_options.UseReadCache)
                StoreReadCacheMemorySizeBytesSummary.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountSummary.Observe(_store.EntryCount);
        }

        public override async Task<Status> WriteBlock(Block block)
        {
            long blockId = block.Number;

            await _eventSender.SendAsync("BlockAdded", block);

            using (WritingBlockDuration.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var result = await session.UpsertAsync(ref blockId, ref block);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                _sessionPool.Return(session);
                return result.Status;
            }
        }

        public override async Task<(bool, Block)> TryGetBlockById(uint blockNum)
        {
            using (BlockReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = (await session.ReadAsync(blockNum)).Complete();
                _sessionPool.Return(session);
                return (status.Found, output);
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