﻿using DeepReader.Storage.Faster.Abis;
using DeepReader.Storage.Faster.ActionTraces;
using DeepReader.Storage.Faster.Blocks;
using DeepReader.Storage.Faster.Transactions;
using DeepReader.Storage.Options;
using DeepReader.Types.EosTypes;
using DeepReader.Types.StorageTypes;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace DeepReader.Storage.Faster
{
    internal sealed class FasterStorage : IStorageAdapter
    {
        private readonly BlockStore _blockStore;
        private readonly TransactionStore _transactionStore;
        private readonly ActionTraceStore _actionTraceStore;
        private readonly AbiStore _abiStore;

        private readonly BlockStoreClient _blockStoreClient;
        private readonly TransactionStoreClient _transactionStoreClient;
        private readonly ActionTraceClient _actionTraceClient;
        private readonly AbiStoreClient _abiStoreClient;

        private FasterStorageOptions _fasterStorageOptions;

        private readonly ParallelOptions _parallelOptions;

        public FasterStorage(
            IOptionsMonitor<FasterStorageOptions> storageOptionsMonitor,
            ITopicEventSender eventSender,
            MetricsCollector metricsCollector)
        {
            _fasterStorageOptions = storageOptionsMonitor.CurrentValue;
            storageOptionsMonitor.OnChange(OnFasterStorageOptionsChanged);

            _blockStore = new BlockStore(_fasterStorageOptions, eventSender, metricsCollector);
            _transactionStore = new TransactionStore(_fasterStorageOptions, eventSender, metricsCollector);
            _actionTraceStore = new ActionTraceStore(_fasterStorageOptions, eventSender, metricsCollector);
            _abiStore = new AbiStore(_fasterStorageOptions, eventSender, metricsCollector);

            _blockStoreClient = new BlockStoreClient();
            _transactionStoreClient = new TransactionStoreClient();
            _actionTraceClient = new ActionTraceClient();
            _abiStoreClient = new AbiStoreClient();

            _parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 6 // TODO, put in config with reasonable name
            };
        }

        private void OnFasterStorageOptionsChanged(FasterStorageOptions newOptions)
        {
            _fasterStorageOptions = newOptions;
        }

        public long BlocksIndexed => _blockStore.BlocksIndexed;
        
        public long TransactionsIndexed => _transactionStore.TransactionsIndexed;

        public async Task StoreBlockAsync(Block block)
        {
            await _blockStore.WriteBlock(block);
            //await _blockStoreClient.WriteBlock(block);
        }

        public async Task StoreTransactionAsync(TransactionTrace transactionTrace)
        {
            await _transactionStoreClient.WriteTransaction(transactionTrace);
        }

        public async Task StoreActionTraceAsync(ActionTrace actionTrace)
        {
            await _actionTraceClient.WriteActionTrace(actionTrace);
        }

        public async Task<(bool, Block)> GetBlockAsync(uint blockNum, bool includeTransactionTraces = false, bool includeActionTraces = false)
        {
            var (found, block) = await _blockStore.TryGetBlockById(blockNum);
            if (found && includeTransactionTraces && block.Transactions?.Count == 0) // if length != 0 values are already loaded and referenced
            {
                var transactionTraceArray = new TransactionTrace[block.TransactionIds.Count];
                // not sure if this is clever or over-parallelized
                await Parallel.ForEachAsync(block.TransactionIds, _parallelOptions, async (transactionId, _) =>
                {
                    if (transactionId != null)
                    {
                        var (foundTrx, transaction) =
                            await _transactionStoreClient.TryGetTransactionTraceById(transactionId);
                        int index;
                        if (foundTrx && (index = block.TransactionIds.IndexOf(transactionId)) >= 0)
                            transactionTraceArray[index] = transaction;
                    }
                });
                block.Transactions = transactionTraceArray.ToList();
            }
            if (found && includeActionTraces && block.Transactions?.FirstOrDefault()?.ActionTraces.Length == 0) // if length != 0 values are already loaded and referenced
            {
                await Parallel.ForEachAsync(block.Transactions, _parallelOptions, async (transaction, _) =>
                {
                    if (transaction != null)
                    {
                        transaction.ActionTraces = new ActionTrace[transaction.ActionTraceIds.Length];
                        // not sure if this is clever or over-parallelized
                        await Parallel.ForEachAsync(transaction.ActionTraceIds, _parallelOptions, async (actionTraceId, _) =>
                        {
                            var (foundAct, action) =
                                await _actionTraceClient.TryGetActionTraceById(actionTraceId);
                            int index;
                            if (foundAct && (index = Array.IndexOf(transaction.ActionTraceIds, actionTraceId)) >= 0)
                                transaction.ActionTraces[index] = action;
                        });

                    }
                });
            }
            return (found, block);
        }

        public async Task<(bool, TransactionTrace)> GetTransactionAsync(string transactionId, bool includeActionTraces = false)
        {
            var (found, transaction) = await _transactionStoreClient.TryGetTransactionTraceById(new Types.Eosio.Chain.TransactionId(transactionId));
            if (found && includeActionTraces && transaction.ActionTraces.Length == 0)  // if length != 0 values are already loaded and referenced
            {
                transaction.ActionTraces = new ActionTrace[transaction.ActionTraceIds.Length];
                // not sure if this is clever or over-parallelized
                await Parallel.ForEachAsync(transaction.ActionTraceIds, _parallelOptions, async (actionTraceId, _) =>
                {
                    var (foundTrx, action) =
                        await _actionTraceClient.TryGetActionTraceById(actionTraceId);
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
            return await _abiStoreClient.TryGetAbiAssembliesById(account);
        }

        public async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence)
        {
            return await _abiStoreClient.TryGetAbiAssemblyByIdAndGlobalSequence(account, globalSequence);
        }

        public async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account)
        {
            return await _abiStoreClient.TryGetActiveAbiAssembly(account);
        }

        public async Task<(bool, ActionTrace)> GetActionTraceAsync(ulong globalSequence)
        {
            return await _actionTraceClient.TryGetActionTraceById(globalSequence);
        }
    }
}
