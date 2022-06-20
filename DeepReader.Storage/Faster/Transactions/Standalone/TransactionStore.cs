using DeepReader.Storage.Faster.Transactions.Base;
using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Serilog;

namespace DeepReader.Storage.Faster.Transactions.Standalone
{
    public class TransactionStore : TransactionStoreBase
    {
        protected readonly FasterKV<TransactionId, TransactionTrace> _store;

        private readonly AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionInput, TransactionOutput,
            TransactionContext, TransactionFunctions>> _sessionPool;

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
                keySerializer = () => new TransactionIdSerializer(),
                valueSerializer = () => new TransactionValueSerializer()
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

            //foreach (var recoverableSession in _store.RecoverableSessions)
            //{
            //    if (recoverableSession.Item2 == "TransactionWriterSession")
            //    {
            //        _transactionWriterSession = _store.For(new TransactionFunctions())
            //            .ResumeSession<TransactionFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
            //    }
            //    else if (recoverableSession.Item2 == "TransactionReaderSession")
            //    {
            //        _transactionReaderSession = _store.For(new TransactionFunctions())
            //            .ResumeSession<TransactionFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
            //    }
            //}

            //_transactionWriterSession ??=
            //    _store.For(new TransactionFunctions()).NewSession<TransactionFunctions>("TransactionSession");
            //_transactionReaderSession ??=
            //    _store.For(new TransactionFunctions()).NewSession<TransactionFunctions>("TransactionReaderSession");

            StoreLogMemorySizeBytesSummary.Observe(_store.Log.MemorySizeBytes);
            if (options.UseReadCache)
                StoreReadCacheMemorySizeBytesSummary.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountSummary.Observe(_store.EntryCount);

            var transactionEvictionObserver = new TransactionEvictionObserver();
            _store.Log.SubscribeEvictions(transactionEvictionObserver);

            if (options.UseReadCache)
                _store.ReadCache.SubscribeEvictions(transactionEvictionObserver);

            _sessionPool = new AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionInput, TransactionOutput, TransactionContext, TransactionFunctions>>(
                logSettings.LogDevice.ThrottleLimit,
                () => _store.For(new TransactionFunctions()).NewSession<TransactionFunctions>("TransactionSession" + Interlocked.Increment(ref _sessionCount)));

            foreach (var recoverableSession in _store.RecoverableSessions)
            {
                _sessionPool.Return(_store.For(new TransactionFunctions())
                    .ResumeSession<TransactionFunctions>(recoverableSession.Item2, out CommitPoint commitPoint));
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

        public override async Task<(bool, TransactionTrace)> TryGetTransactionTraceById(Types.Eosio.Chain.TransactionId transactionId)
        {
            using (TransactionReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = (await session.ReadAsync(new TransactionId(transactionId))).Complete();
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
