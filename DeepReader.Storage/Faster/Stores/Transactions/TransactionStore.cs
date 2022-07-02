using DeepReader.Storage.Faster.StoreBase;
using DeepReader.Storage.Faster.StoreBase.Standalone;
using DeepReader.Storage.Options;
using DeepReader.Types.Eosio.Chain;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Serilog;
using TransactionTrace = DeepReader.Types.StorageTypes.TransactionTrace;

namespace DeepReader.Storage.Faster.Stores.Transactions
{
    public class TransactionStore : TransactionStoreBase
    {
        protected readonly FasterKV<TransactionId, TransactionTrace> Store;

        private readonly AsyncPool<ClientSession<TransactionId, TransactionTrace, Input, TransactionTrace,
            KeyValueContext, StandaloneFunctions<TransactionId, TransactionTrace>>> _sessionPool;

        public TransactionStore(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            if (!Options.TransactionStoreDir.EndsWith("/"))
                options.TransactionStoreDir += "/";

            // Create files for storing data
            var log = Devices.CreateLogDevice(Options.TransactionStoreDir + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(Options.TransactionStoreDir + "hlog.obj.log");

            // Define settings for log
            var logSettings = new LogSettings
            {
                LogDevice = log,
                ObjectLogDevice = objlog,
                ReadCacheSettings = Options.UseReadCache ? new ReadCacheSettings() : null,
                // to calculate below:
                // 12 = 00001111 11111111 = 4095 = 4K
                // 34 = 00000011 11111111 11111111 11111111 11111111 = 17179869183 = 16G
                PageSizeBits = 14, // (16K pages)
                MemorySizeBits = 33 // (8G memory for main log)
            };

            // Define serializers; otherwise FASTER will use the slower DataContract
            // Needed only for class keys/values
            var serializerSettings = new SerializerSettings<TransactionId, TransactionTrace>
            {
                keySerializer = () => new KeySerializer<TransactionId, TransactionId>(),
                valueSerializer = () => new ValueSerializer<TransactionTrace>()
            };

            var checkPointsDir = Options.TransactionStoreDir + "checkpoints";

            var checkpointManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(checkPointsDir), true);

            Store = new FasterKV<TransactionId, TransactionTrace>(
                size: Options.MaxTransactionsCacheEntries, // Cache Lines for Transactions
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointManager = checkpointManager },
                serializerSettings: serializerSettings,
                comparer: new TransactionId()
            );

            if (Directory.Exists(checkPointsDir))
            {
                try
                {
                    Log.Information("Recovering TransactionStore");
                    Store.Recover(1);
                    Log.Information("TransactionStore recovered");
                }
                catch (Exception e)
                {
                    Log.Error(e, "");
                    throw;
                }
            }


            TypeStoreLogMemorySizeBytesSummary.Observe(Store.Log.MemorySizeBytes);
            if (options.UseReadCache)
                TypeStoreReadCacheMemorySizeBytesSummary.Observe(Store.ReadCache.MemorySizeBytes);
            TypeStoreEntryCountSummary.Observe(Store.EntryCount);

            var transactionEvictionObserver = new PooledObjectEvictionObserver<TransactionId, TransactionTrace>();
            Store.Log.SubscribeEvictions(transactionEvictionObserver);

            if (options.UseReadCache)
                Store.ReadCache.SubscribeEvictions(transactionEvictionObserver);

            _sessionPool = new AsyncPool<ClientSession<TransactionId, TransactionTrace, Input, TransactionTrace, KeyValueContext, StandaloneFunctions<TransactionId, TransactionTrace>>>(
                logSettings.LogDevice.ThrottleLimit,
                () => Store.For(new StandaloneFunctions<TransactionId, TransactionTrace>()).NewSession<StandaloneFunctions<TransactionId, TransactionTrace>>("TransactionSession" + Interlocked.Increment(ref SessionCount)));

            foreach (var recoverableSession in Store.RecoverableSessions)
            {
                _sessionPool.Return(Store.For(new StandaloneFunctions<TransactionId, TransactionTrace>())
                    .ResumeSession<StandaloneFunctions<TransactionId, TransactionTrace>>(recoverableSession.Item2, out CommitPoint commitPoint));
                SessionCount++;
            }

            new Thread(CommitThread).Start();
        }

        public long TransactionsIndexed => Store.EntryCount;

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {
            TypeStoreLogMemorySizeBytesSummary.Observe(Store.Log.MemorySizeBytes);
            if (Options.UseReadCache)
                TypeStoreReadCacheMemorySizeBytesSummary.Observe(Store.ReadCache.MemorySizeBytes);
            TypeStoreEntryCountSummary.Observe(Store.EntryCount);
        }

        public override async Task<Status> WriteTransaction(TransactionTrace transaction)
        {
            var transactionId = new TransactionId(transaction.Id);

            await EventSender.SendAsync("TransactionAdded", transaction);

            using (TypeWritingTransactionDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var result = await session.UpsertAsync(ref transactionId, ref transaction);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                _sessionPool.Return(session);
                return result.Status;
            }
        }

        public override async Task<(bool, TransactionTrace)> TryGetTransactionTraceById(TransactionId transactionId)
        {
            using (TypeTransactionReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = (await session.ReadAsync(new TransactionId(transactionId))).Complete();
                _sessionPool.Return(session);
                return (status.Found, output);
            }
        }

        private void CommitThread()
        {
            if (Options.LogCheckpointInterval is null or 0)
                return;

            int logCheckpointsTaken = 0;

            while (true)
            {
                try
                {
                    Thread.Sleep(Options.LogCheckpointInterval.Value);

                    using (TypeStoreTakeLogCheckpointDurationSummary.NewTimer())
                        Store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver, true).GetAwaiter().GetResult();

                    if (Options.FlushAfterCheckpoint)
                    {
                        using (TypeStoreFlushAndEvictLogDurationSummary.NewTimer())
                            Store.Log.FlushAndEvict(true);
                    }

                    if (logCheckpointsTaken % Options.IndexCheckpointMultiplier == 0)
                    {
                        using (TypeStoreTakeIndexCheckpointDurationSummary.NewTimer())
                            Store.TakeIndexCheckpointAsync().GetAwaiter().GetResult();
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
