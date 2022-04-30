using DeepReader.Storage;
using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.Queries
{
    internal class TransactionQuery
    {
        public async Task<FlattenedTransactionTrace?> GetTransaction(string transaction_id, [Service]IStorageAdapter storage)
        {
            var (found, transaction) = await storage.GetTransactionAsync(transaction_id);
            if (found)
                return transaction;
            return null;
        }
    }
}