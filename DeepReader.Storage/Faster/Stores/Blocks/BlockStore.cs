using DeepReader.Storage.Faster.Options;
using DeepReader.Storage.Faster.StoreBase;
using DeepReader.Storage.Faster.StoreBase.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Serilog;

namespace DeepReader.Storage.Faster.Stores.Blocks
{
    public class BlockStore : BlockStoreBase 
    {
        private FasterStandaloneOptions StandaloneOptions => (FasterStandaloneOptions)Options;

        protected readonly FasterKV<long, Block> Store;

        private readonly AsyncPool<ClientSession<long, Block, Block, Block, KeyValueContext,
            StandaloneFunctions<long, Block>>> _sessionPool;

        public new long BlocksIndexed => Store.EntryCount;

        public BlockStore(IFasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            if (!StandaloneOptions.BlockStoreDir.EndsWith("/"))
                StandaloneOptions.BlockStoreDir += "/";


            // Create files for storing data
            var log = Devices.CreateLogDevice(StandaloneOptions.BlockStoreDir + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(StandaloneOptions.BlockStoreDir + "hlog.obj.log");

            // Define settings for log
            var logSettings = new LogSettings
            {
                LogDevice = log,
                ObjectLogDevice = objlog,
                ReadCacheSettings = StandaloneOptions.UseReadCache ? new ReadCacheSettings() : null,
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

            var checkPointsDir = StandaloneOptions.BlockStoreDir + "checkpoints";

            var checkpointManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(checkPointsDir), true);


            Store = new FasterKV<long, Block>(
                size: StandaloneOptions.MaxBlocksCacheEntries, // Cache Lines for Blocks
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
                    Store.Recover(1);
                    Log.Information("BlockStore recovered");
                }
                catch (Exception e)
                {
                    Log.Error(e, "");
                    throw;
                }
            }

            TypeStoreLogMemorySizeBytesSummary.Observe(Store.Log.MemorySizeBytes);
            if (StandaloneOptions.UseReadCache)
                TypeStoreReadCacheMemorySizeBytesSummary.Observe(Store.ReadCache.MemorySizeBytes);// must be optional
            TypeStoreEntryCountSummary.Observe(Store.EntryCount);
            MetricsCollector.CollectMetricsHandler += CollectObservableMetrics;

            var blockEvictionObserver = new PooledObjectEvictionObserver<long, Block>();
            Store.Log.SubscribeEvictions(blockEvictionObserver);

            if (StandaloneOptions.UseReadCache)
                Store.ReadCache.SubscribeEvictions(blockEvictionObserver);

            _sessionPool = new AsyncPool<ClientSession<long, Block, Block, Block, KeyValueContext, StandaloneFunctions<long, Block>>>(
                logSettings.LogDevice.ThrottleLimit,
                () => Store.For(new StandaloneFunctions<long, Block>()).NewSession<StandaloneFunctions<long, Block>>("BlockSession" + Interlocked.Increment(ref SessionCount)));

            foreach (var recoverableSession in Store.RecoverableSessions)
            {
                _sessionPool.Return(Store.For(new StandaloneFunctions<long, Block>())
                    .ResumeSession<StandaloneFunctions<long, Block>>(recoverableSession.Item2, out CommitPoint commitPoint));
                SessionCount++;
            }

            //new Thread(CommitThread).Start();
        }

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {
            TypeStoreLogMemorySizeBytesSummary.Observe(Store.Log.MemorySizeBytes);
            if (StandaloneOptions.UseReadCache)
                TypeStoreReadCacheMemorySizeBytesSummary.Observe(Store.ReadCache.MemorySizeBytes);
            TypeStoreEntryCountSummary.Observe(Store.EntryCount);
        }

        public override async Task<Status> WriteBlock(Block block)
        {
            long blockId = block.Number;

            await EventSender.SendAsync("BlockAdded", block);

            using (TypeWritingBlockDuration.NewTimer())
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
            using (TypeBlockReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = (await session.ReadAsync(blockNum)).Complete();
                _sessionPool.Return(session);
                return (status.Found, output);
            }
        }

        //private void CommitThread()
        //{
        //    if (StandaloneOptions.LogCheckpointInterval is null or 0)
        //        return;

        //    int logCheckpointsTaken = 0;

        //    while (true)
        //    {
        //        try
        //        {
        //            Thread.Sleep(StandaloneOptions.LogCheckpointInterval.Value);

        //            using (TypeStoreTakeLogCheckpointDurationSummary.NewTimer())
        //                Store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver, true).GetAwaiter().GetResult();

        //            if (StandaloneOptions.FlushAfterCheckpoint)
        //            {
        //                using (TypeStoreFlushAndEvictLogDurationSummary.NewTimer())
        //                    Store.Log.FlushAndEvict(true);
        //            }

        //            if (logCheckpointsTaken % StandaloneOptions.IndexCheckpointMultiplier == 0)
        //            {
        //                using (TypeStoreTakeIndexCheckpointDurationSummary.NewTimer())
        //                    Store.TakeIndexCheckpointAsync().GetAwaiter().GetResult();
        //            }

        //            logCheckpointsTaken++;
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex, "");
        //        }
        //    }
        //}

        public async Task Commit()
        {
            int logCheckpointsTaken = 0;

            try
            {
                using (TypeStoreTakeLogCheckpointDurationSummary.NewTimer())
                    Store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver, true).GetAwaiter().GetResult();

                if (StandaloneOptions.FlushAfterCheckpoint)
                {
                    using (TypeStoreFlushAndEvictLogDurationSummary.NewTimer())
                        Store.Log.FlushAndEvict(true);
                }

                if (logCheckpointsTaken % StandaloneOptions.IndexCheckpointMultiplier == 0)
                {
                    using (TypeStoreTakeIndexCheckpointDurationSummary.NewTimer())
                        await Store.TakeIndexCheckpointAsync();
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