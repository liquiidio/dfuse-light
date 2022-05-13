using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Storage
{
    public interface IStorageAdapter
    {
        long BlocksIndexed { get; }

        long TransactionsIndexed { get; }

        Task StoreBlockAsync(FlattenedBlock block);

        Task StoreTransactionAsync(FlattenedTransactionTrace transactionTrace);

        Task<(bool, FlattenedBlock)> GetBlockAsync(uint blockNum, bool includeTransactionTraces = false);

        Task<(bool, FlattenedTransactionTrace)> GetTransactionAsync(string transactionId);
    }
}