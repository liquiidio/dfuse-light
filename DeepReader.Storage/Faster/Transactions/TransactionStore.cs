using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Sentry;
using Serilog;

namespace DeepReader.Storage.Faster.Transactions
{
    public sealed class TransactionStore
    {
        private readonly FasterKV<TransactionId, TransactionTrace> _store;

        private readonly ClientSession<TransactionId, TransactionTrace, TransactionInput, TransactionOutput, TransactionContext, TransactionFunctions> _transactionReaderSession;
        private readonly ClientSession<TransactionId, TransactionTrace, TransactionInput, TransactionOutput, TransactionContext, TransactionFunctions> _transactionWriterSession;

        private readonly FasterStorageOptions _options;

        private readonly ITopicEventSender _eventSender;

        private static readonly Histogram WritingTransactionDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_write_transaction_duration", "Histogram of time to store transactions to Faster");
        private static readonly Histogram StoreLogMemorySizeBytesHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_transaction_store_log_memory_size_bytes", "Histogram of the faster transaction store log memory size in bytes");
        private static readonly Histogram StoreReadCacheMemorySizeBytesHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_transaction_store_read_cache_memory_size_bytes", "Histogram of the faster transaction store read cache memory size in bytes");
        private static readonly Histogram StoreEntryCountHistogram =
           Metrics.CreateHistogram("deepreader_storage_faster_transaction_store_read_cache_memory_size_bytes", "Histogram of the faster transaction store entry count");
        private static readonly Histogram StoreTakeFullCheckpointDurationHistogram =
          Metrics.CreateHistogram("deepreader_storage_faster_transaction_store_take_full_checkpoint_duration", "Histogram of time to take a full checkpoint of faster transaction store");
        private static readonly Histogram StoreFlushAndEvictLogDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_transaction_store_log_flush_and_evict_duration", "Histogram of time to flush and evict faster transaction store");
        private static readonly Histogram TransactionReaderSessionReadDurationHistogram =
          Metrics.CreateHistogram("deepreader_storage_faster_transaction_get_by_id_duration", "Histogram of time to try get transaction trace by id");

        public TransactionStore(FasterStorageOptions options, ITopicEventSender eventSender)
        {
            _options = options;
            _eventSender = eventSender;

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
                PageSizeBits = 12, // (4K pages)
                MemorySizeBits = 34 // (16G memory for main log)
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
                Log.Information("Recovering TransactionStore");
                _store.Recover(1);
                Log.Information("TransactionStore recovered");
            }

            foreach (var recoverableSession in _store.RecoverableSessions)
            {
                if (recoverableSession.Item2 == "TransactionWriterSession")
                {
                    _transactionWriterSession = _store.For(new TransactionFunctions())
                        .ResumeSession<TransactionFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
                }
                else if (recoverableSession.Item2 == "TransactionReaderSession")
                {
                    _transactionReaderSession = _store.For(new TransactionFunctions())
                        .ResumeSession<TransactionFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
                }
            }

            _transactionWriterSession ??=
                _store.For(new TransactionFunctions()).NewSession<TransactionFunctions>("TransactionWriterSession");
            _transactionReaderSession ??=
                _store.For(new TransactionFunctions()).NewSession<TransactionFunctions>("TransactionReaderSession");

            StoreLogMemorySizeBytesHistogram.Observe(_store.Log.MemorySizeBytes);
            if (options.UseReadCache)
                StoreReadCacheMemorySizeBytesHistogram.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountHistogram.Observe(_store.EntryCount);

            // TODO, for some reason I need to manually call the Init
            SentrySdk.Init("https://b4874920c4484212bcc323e9deead2e9@sentry.noodles.lol/2");

            new Thread(CommitThread).Start();
        }

        public async Task<Status> WriteTransaction(TransactionTrace transaction)
        {
            var transactionId = new TransactionId(transaction.Id);

            await _eventSender.SendAsync("TransactionAdded", transaction);

            using (WritingTransactionDurationHistogram.NewTimer())
            {
                var result = await _transactionWriterSession.UpsertAsync(ref transactionId, ref transaction);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                return result.Status;
            }
        }

        public async Task<(bool, TransactionTrace)> TryGetTransactionTraceById(Types.Eosio.Chain.TransactionId transactionId)
        {
            using (TransactionReaderSessionReadDurationHistogram.NewTimer())
            {
                var (status, output) = (await _transactionReaderSession.ReadAsync(new TransactionId(transactionId))).Complete();
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
