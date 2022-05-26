using DeepReader.Apis.GraphQl.DataLoaders;
//using DeepReader.Storage;
using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.Queries
{
    internal class TransactionQuery
    {
        //public async Task<TransactionTrace?> GetTransaction(string transaction_id, [Service]IStorageAdapter storage)
        public Task<TransactionTrace> GetTransaction(string transaction_id, TransactionByIdDataLoader dataLoader, CancellationToken cancellationToken)
        {
            //var (found, transaction) = await storage.GetTransactionAsync(transaction_id);
            //if (found)
            //    return transaction;
            //return null;
            return dataLoader.LoadAsync(transaction_id, cancellationToken);
        }
    }
}