using DeepReader.Types.StorageTypes;
using System.Reflection;

namespace DeepReader.Storage
{
    public interface IStorageAdapter
    {
        Task StoreBlockAsync(Block block);

        Task StoreTransactionAsync(TransactionTrace transactionTrace);

        Task StoreActionTraceAsync(ActionTrace actionTrace);

        Task<(bool, Block)> GetBlockAsync(uint blockNum, bool includeTransactionTraces = false, bool includeActionTraces = false);

        Task<(bool, FlattenedTransactionTrace)> GetTransactionAsync(string transactionId);

        Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly);

        Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account);

        Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence);

        Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account);

        Task<(bool, TransactionTrace)> GetTransactionAsync(string transactionId, bool includeActionTraces = false);
    }
}
