using DeepReader.Types.StorageTypes;

namespace DeepReader.Storage
{
    public interface IStorageAdapter
    {
        Task StoreBlockAsync(Block block);

        Task StoreTransactionAsync(TransactionTrace transactionTrace);

        Task StoreActionTraceAsync(ActionTrace actionTrace);

        Task<(bool, Block)> GetBlockAsync(uint blockNum, bool includeTransactionTraces = false, bool includeActionTraces = false);

        Task<(bool, TransactionTrace)> GetTransactionAsync(string transactionId, bool includeActionTraces = false);
    }
}
