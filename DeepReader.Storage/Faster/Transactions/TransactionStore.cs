using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.FlattenedTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.Transactions
{
    public class TransactionStore
    {
        private FasterKV<TransactionId, FlattenedTransactionTrace> store;

        private bool useReadCache = false;

        private readonly ClientSession<TransactionId, FlattenedTransactionTrace, TransactionInput, TransactionOutput, TransactionContext, TransactionFunctions> _transactionStoreSession;

        public TransactionStore()
        {

            // Create files for storing data
            var path = Path.GetTempPath() + "ClassCache/";
            var log = Devices.CreateLogDevice(path + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(Path.GetTempPath() + "hlog.obj.log");

            // Define settings for log
            var logSettings = new LogSettings
            {
                LogDevice = log,
                ObjectLogDevice = objlog,
                ReadCacheSettings = useReadCache ? new ReadCacheSettings() : null,
                // Uncomment below for low memory footprint demo
                // PageSizeBits = 12, // (4K pages)
                // MemorySizeBits = 20 // (1M memory for main log)
            };

            // Define serializers; otherwise FASTER will use the slower DataContract
            // Needed only for class keys/values
            var serializerSettings = new SerializerSettings<TransactionId, FlattenedTransactionTrace>
            {
                keySerializer = () => new TransactionIdSerializer(),
                valueSerializer = () => new TransactionValueSerializer()
            };

            store = new FasterKV<TransactionId, FlattenedTransactionTrace>(
                size: 1L << 20,
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointDir = path },
                serializerSettings: serializerSettings,
                comparer: new TransactionId()
            );

            _transactionStoreSession = store.For(new TransactionFunctions()).NewSession<TransactionFunctions>();
        }

        public async Task<Status> WriteTransaction(FlattenedTransactionTrace transaction)
        {
            var transactionId = new TransactionId(transaction.Id);
            return (await _transactionStoreSession.UpsertAsync(ref transactionId, ref transaction)).Complete();
        }

    }
}
