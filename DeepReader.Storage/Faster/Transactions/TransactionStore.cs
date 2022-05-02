using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Storage.Options;
using DeepReader.Types.FlattenedTypes;
using FASTER.core;
using Prometheus;
using Serilog;

namespace DeepReader.Storage.Faster.Transactions
{
    public class TransactionStore
    {
        private readonly FasterKV<TransactionId, FlattenedTransactionTrace> _store;

        private readonly ClientSession<TransactionId, FlattenedTransactionTrace, TransactionInput, TransactionOutput, TransactionContext, TransactionFunctions> _transactionReaderSession;
        private readonly ClientSession<TransactionId, FlattenedTransactionTrace, TransactionInput, TransactionOutput, TransactionContext, TransactionFunctions> _transactionWriterSession;

        private FasterStorageOptions _options;

        private static readonly Histogram WritingTransactionDuration = Metrics.CreateHistogram("deepreader_storage_faster_write_transaction_duration", "Histogram of time to store transactions to Faster");

        public TransactionStore(FasterStorageOptions options)
        {
            _options = options;

            if(!_options.TransactionStoreDir.EndsWith("/"))
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
            var serializerSettings = new SerializerSettings<TransactionId, FlattenedTransactionTrace>
            {
                keySerializer = () => new TransactionIdSerializer(),
                valueSerializer = () => new TransactionValueSerializer()
            };

            var checkPointsDir = _options.TransactionStoreDir + "checkpoints";

            var checkpointManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(checkPointsDir), true);

            _store = new FasterKV<TransactionId, FlattenedTransactionTrace>(
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

            new Thread(CommitThread).Start();
        }

        public async Task<Status> WriteTransaction(FlattenedTransactionTrace transaction)
        {
            var transactionId = new TransactionId(transaction.Id);

            using (WritingTransactionDuration.NewTimer())
            {
                var result = await _transactionWriterSession.UpsertAsync(ref transactionId, ref transaction);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                return result.Status;
            }
        }

        public async Task<(bool, FlattenedTransactionTrace)> TryGetTransactionTraceById(Types.Eosio.Chain.TransactionId transactionId)
        {
            var (status, output) = (await _transactionReaderSession.ReadAsync(new TransactionId(transactionId))).Complete();
            return (status.Found, output.Value);
        }

        private void CommitThread()
        {
            if (_options.CheckpointInterval is null or 0) 
                return;
            
            while (true)
            {
                Thread.Sleep(_options.CheckpointInterval.Value);

                // Take log-only checkpoint (quick - no index save)
                //store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver).GetAwaiter().GetResult();

                // Take index + log checkpoint (longer time)
                _store.TakeFullCheckpointAsync(CheckpointType.FoldOver).GetAwaiter().GetResult();
                _store.Log.FlushAndEvict(true);
            }
        }
    }
}
