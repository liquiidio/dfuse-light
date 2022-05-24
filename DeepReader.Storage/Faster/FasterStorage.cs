using DeepReader.Storage.Faster.Abis;
using DeepReader.Storage.Faster.ActionTraces;
using DeepReader.Storage.Faster.Blocks;
using DeepReader.Storage.Faster.Transactions;
using DeepReader.Storage.Options;
using DeepReader.Types.EosTypes;
using DeepReader.Types.StorageTypes;
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
        private readonly ActionTraceStore _actionTraceStore;
        private readonly AbiStore _abiStore;

        private FasterStorageOptions _fasterStorageOptions;

        private ParallelOptions _parallelOptions;

        public FasterStorage(
            IOptionsMonitor<FasterStorageOptions> storageOptionsMonitor,
            ITopicEventSender eventSender,
            MetricsCollector metricsCollector)
        {
            _fasterStorageOptions = storageOptionsMonitor.CurrentValue;
            storageOptionsMonitor.OnChange(OnFasterStorageOptionsChanged);

            _blockStore = new BlockStore(_fasterStorageOptions, eventSender);
            _transactionStore = new TransactionStore(_fasterStorageOptions, eventSender);
            _actionTraceStore = new ActionTraceStore(_fasterStorageOptions, eventSender);
            _abiStore = new AbiStore(_fasterStorageOptions, eventSender);

            _parallelOptions = new()
            {
                MaxDegreeOfParallelism = 6 // TODO, put in config with reasonable name
            };
        }

        private void OnFasterStorageOptionsChanged(FasterStorageOptions newOptions)
        {
            _fasterStorageOptions = newOptions;
        }

        public async Task StoreBlockAsync(Block block)
        {
            await _blockStore.WriteBlock(block);
        }

        public async Task StoreTransactionAsync(TransactionTrace transactionTrace)
        {
            await _transactionStore.WriteTransaction(transactionTrace);
        }

        public async Task StoreActionTraceAsync(ActionTrace actionTrace)
        {
            await _actionTraceStore.WriteActionTrace(actionTrace);
        }

        public async Task<(bool, Block)> GetBlockAsync(uint blockNum, bool includeTransactionTraces = false, bool includeActionTraces = false)
        {
            var (found, block) = await _blockStore.TryGetBlockById(blockNum);
            if (found && includeTransactionTraces && block.Transactions.Length == 0) // if length != 0 values are already loaded and referenced
            {
                block.Transactions = new TransactionTrace[block.TransactionIds.Length];
                // not sure if this is clever or over-parallelized
                await Parallel.ForEachAsync(block.TransactionIds, _parallelOptions, async (transactionId, _) =>
                {
                    var (foundTrx, transaction) =
                        await _transactionStore.TryGetTransactionTraceById(transactionId);
                    var index = 0;
                    if (foundTrx && (index = Array.IndexOf(block.TransactionIds, transactionId)) >= 0)
                        block.Transactions[index] = transaction;
                });
            }
            if (found && includeActionTraces && block.Transactions?.FirstOrDefault()?.ActionTraces.Length == 0) // if length != 0 values are already loaded and referenced
            {
                await Parallel.ForEachAsync(block.Transactions, _parallelOptions, async (transaction, _) =>
                {
                    if(transaction != null)
                    {
                        transaction.ActionTraces = new ActionTrace[transaction.ActionTraceIds.Length];
                        // not sure if this is clever or over-parallelized
                        await Parallel.ForEachAsync(transaction.ActionTraceIds, _parallelOptions, async (actionTraceId, _) =>
                        {
                            var (found, action) =
                                await _actionTraceStore.TryGetActionTraceById(actionTraceId);
                            var index = 0;
                            try
                            {
                                if (found && (index = Array.IndexOf(transaction.ActionTraceIds, actionTraceId)) >= 0)
                                    transaction.ActionTraces[index] = action;
                            }
                            catch (Exception ex)
                            {
                                string test = index.ToString();
                                string a = transaction.ActionTraces.Length.ToString();
                            }
                        });
                    }
                });
            }
            return (found, block);
        }

        public async Task<(bool, TransactionTrace)> GetTransactionAsync(string transactionId, bool includeActionTraces = false)
        {
            var (found, transaction) = await _transactionStore.TryGetTransactionTraceById(new Types.Eosio.Chain.TransactionId(transactionId));
            if (found && includeActionTraces && transaction.ActionTraces.Length == 0)  // if length != 0 values are already loaded and referenced
            {
                transaction.ActionTraces = new ActionTrace[transaction.ActionTraceIds.Length];
                // not sure if this is clever or over-parallelized
                await Parallel.ForEachAsync(transaction.ActionTraceIds, _parallelOptions, async (actionTraceId, _) =>
                {
                    var (foundTrx, action) =
                        await _actionTraceStore.TryGetActionTraceById(actionTraceId);
                    if (foundTrx)
                        transaction.ActionTraces[Array.IndexOf(transaction.ActionTraceIds, actionTraceId)] = action;
                });
            }
            return (found, transaction);
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

        public async Task<(bool, ActionTrace)> GetActionTraceAsync(ulong globalSequence)
        {
            return await _actionTraceStore.TryGetActionTraceById(globalSequence);
        }
    }
}
