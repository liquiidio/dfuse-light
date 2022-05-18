using DeepReader.Storage.Faster.Abis;
using DeepReader.Storage.Faster.Blocks;
using DeepReader.Storage.Faster.Transactions;
using DeepReader.Storage.Options;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.FlattenedTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace DeepReader.Storage.Faster
{
    internal class FasterStorage : IStorageAdapter
    {
        private readonly BlockStore _blockStore;
        private readonly TransactionStore _transactionStore;
        private readonly AbiStore _abiStore;

        private FasterStorageOptions _fasterStorageOptions;

        public FasterStorage(IOptionsMonitor<FasterStorageOptions> storageOptionsMonitor, ITopicEventSender eventSender)
        {
            _fasterStorageOptions = storageOptionsMonitor.CurrentValue;
            storageOptionsMonitor.OnChange(OnFasterStorageOptionsChanged);

            _blockStore = new BlockStore(_fasterStorageOptions, eventSender);
            _transactionStore = new TransactionStore(_fasterStorageOptions, eventSender);
            _abiStore = new AbiStore(_fasterStorageOptions, eventSender);
        }

        private void OnFasterStorageOptionsChanged(FasterStorageOptions newOptions)
        {
            _fasterStorageOptions = newOptions;
        }

        public async Task StoreBlockAsync(FlattenedBlock block)
        {
            await _blockStore.WriteBlock(block);
        }

        public async Task StoreTransactionAsync(FlattenedTransactionTrace transactionTrace)
        { 
            await _transactionStore.WriteTransaction(transactionTrace);
        }

        public async Task<(bool,FlattenedBlock)> GetBlockAsync(uint blockNum, bool includeTransactionTraces = false)
        {
            var (found, block) = await _blockStore.TryGetBlockById(blockNum);
            if (found && includeTransactionTraces)
            {
                block.Transactions = new FlattenedTransactionTrace[block.TransactionIds.Length];
                int i = 0;
                foreach (var transactionId in block.TransactionIds)
                {
                    var (foundTrx, trx) =
                        await _transactionStore.TryGetTransactionTraceById(transactionId);
                    if(foundTrx)
                        block.Transactions[i++] = trx;
                }

                block.TransactionIds = Array.Empty<Types.Eosio.Chain.TransactionId>();
            }
            return (found, block);
        }

        public async Task<(bool, FlattenedTransactionTrace)> GetTransactionAsync(string transactionId)
        {
            return await _transactionStore.TryGetTransactionTraceById(new Types.Eosio.Chain.TransactionId(transactionId));
        }

        public async Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly)
        {
            await _abiStore.UpsertAbi(account, globalSequence, assembly);
        }

        public async Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account)
        {
            return await _abiStore.TryGetAbiAssembliesById(account);
        }

        public async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence)
        {
            return await _abiStore.TryGetAbiAssemblyByIdAndGlobalSequence(account, globalSequence);
        }

        public async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account)
        {
            return await _abiStore.TryGetActiveAbiAssembly(account);
        }
    }
}
