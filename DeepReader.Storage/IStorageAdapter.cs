using DeepReader.Storage.Faster.Abis;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.FlattenedTypes;
using System.Reflection;

namespace DeepReader.Storage
{
    public interface IStorageAdapter
    {
        Task StoreBlockAsync(FlattenedBlock block);

        Task StoreTransactionAsync(FlattenedTransactionTrace transactionTrace);

        Task<(bool, FlattenedBlock)> GetBlockAsync(uint blockNum, bool includeTransactionTraces = false);

        Task<(bool, FlattenedTransactionTrace)> GetTransactionAsync(string transactionId);

        Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly);

        Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account);

        Task<(bool, KeyValuePair<ulong, Assembly>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence);

        Task<(bool, KeyValuePair<ulong, Assembly>)> TryGetActiveAbiAssembly(Name account);

    }
}
