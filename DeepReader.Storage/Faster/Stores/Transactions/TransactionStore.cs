using DeepReader.Storage.Faster.Base;
using DeepReader.Storage.Faster.Base.Standalone;
using DeepReader.Storage.Faster.Test;
using DeepReader.Storage.Options;
using DeepReader.Types.Eosio.Chain;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Serilog;
using TransactionTrace = DeepReader.Types.StorageTypes.TransactionTrace;

namespace DeepReader.Storage.Faster.Transactions
{
    public class TransactionStore : TransactionStoreBase
    {
        protected readonly FasterKV<TransactionId, TransactionTrace> _store;

        private readonly AsyncPool<ClientSession<TransactionId, TransactionTrace, Input, TransactionTrace,
            KeyValueContext, StandaloneFunctions<TransactionId, TransactionTrace>>> _sessionPool;

        private int _sessionCount;

        public TransactionStore(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            if (!_options.TransactionStoreDir.EndsWith("/"))
                options.TransactionStoreDir += "/";

            // Create files for storing data
            var log = Devices.CreateLogDevice(_options.TransactionStoreDir + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(_options.TransactionStoreDir + "hlog.obj.log");

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
            var serializerSettings = new SerializerSettings<TransactionId, TransactionTrace>
            {
                keySerializer = () => new KeySerializer<TransactionId, TransactionId>(),
                valueSerializer = () => new ValueSerializer<TransactionTrace>()
            };

            var checkPointsDir = _options.TransactionStoreDir + "checkpoints";

            var checkpointManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(checkPointsDir), true);

            _store = new FasterKV<TransactionId, TransactionTrace>(
                size: _options.MaxTransactionsCacheEntries, // Cache Lines for Transactions
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
                    _store.Recover(1);
                    Log.Information("TransactionStore recovered");
                }
                catch (Exception e)
                {
                    Log.Error(e, "");
                    throw;
                }
            }


            StoreLogMemorySizeBytesSummary.Observe(_store.Log.MemorySizeBytes);
            if (options.UseReadCache)
                StoreReadCacheMemorySizeBytesSummary.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountSummary.Observe(_store.EntryCount);

            var transactionEvictionObserver = new PooledObjectEvictionObserver<TransactionId, TransactionTrace>();
            _store.Log.SubscribeEvictions(transactionEvictionObserver);

            if (options.UseReadCache)
                _store.ReadCache.SubscribeEvictions(transactionEvictionObserver);

            _sessionPool = new AsyncPool<ClientSession<TransactionId, TransactionTrace, Input, TransactionTrace, KeyValueContext, StandaloneFunctions<TransactionId, TransactionTrace>>>(
                logSettings.LogDevice.ThrottleLimit,
                () => _store.For(new StandaloneFunctions<TransactionId, TransactionTrace>()).NewSession<StandaloneFunctions<TransactionId, TransactionTrace>>("TransactionSession" + Interlocked.Increment(ref _sessionCount)));

            foreach (var recoverableSession in _store.RecoverableSessions)
            {
                _sessionPool.Return(_store.For(new StandaloneFunctions<TransactionId, TransactionTrace>())
                    .ResumeSession<StandaloneFunctions<TransactionId, TransactionTrace>>(recoverableSession.Item2, out CommitPoint commitPoint));
                _sessionCount++;
            }

            new Thread(CommitThread).Start();
        }

        public long TransactionsIndexed => _store.EntryCount;

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {
            StoreLogMemorySizeBytesSummary.Observe(_store.Log.MemorySizeBytes);
            if (_options.UseReadCache)
                StoreReadCacheMemorySizeBytesSummary.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountSummary.Observe(_store.EntryCount);
        }

        public override async Task<Status> WriteTransaction(TransactionTrace transaction)
        {
            var transactionId = new TransactionId(transaction.Id);

            await _eventSender.SendAsync("TransactionAdded", transaction);

            using (WritingTransactionDurationSummary.NewTimer())
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
            using (TransactionReaderSessionReadDurationSummary.NewTimer())
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
