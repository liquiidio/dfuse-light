using DeepReader.Storage.Faster.Abis;
using DeepReader.Types.EosTypes;
using DeepReader.Types.StorageTypes;
using System.Reflection;

 namespace DeepReader.Storage
{
    public interface IStorageAdapter
    {
        long BlocksIndexed { get; }

        long TransactionsIndexed { get; }

        Task StoreBlockAsync(Block block);

        Task StoreTransactionAsync(TransactionTrace transactionTrace);

        Task StoreActionTraceAsync(ActionTrace actionTrace);

        Task<(bool, Block)> GetBlockAsync(uint blockNum, bool includeTransactionTraces = false, bool includeActionTraces = false);

        Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly);

        Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account);

        Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence);

        Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account);

        Task<(bool, TransactionTrace)> GetTransactionAsync(string transactionId, bool includeActionTraces = false);

        Task<(bool, ActionTrace)> GetActionTraceAsync(ulong globalSequence);
    }
}