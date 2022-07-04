using DeepReader.Types.EosTypes;
using DeepReader.Types.StorageTypes;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.Options;
using System.Reflection;
using DeepReader.Storage.Faster.Stores.ActionTraces;
using DeepReader.Storage.Faster.Stores.Blocks;
using DeepReader.Storage.Faster.Stores.Transactions;
using DeepReader.Storage.Faster.Options;
using DeepReader.Storage.Faster.Stores.Abis;
using DeepReader.Storage.Faster.Stores.Abis.Custom;

namespace DeepReader.Storage.Faster
{
    internal sealed class FasterStorage : IStorageAdapter
    {
        private readonly BlockStoreBase _blockStore;
        private readonly TransactionStoreBase _transactionStore;
        private readonly ActionTraceStoreBase _actionTraceStore;
        private readonly AbiStoreBase _abiStore;

        private readonly TransactionStoreClient _transactionStoreClient;


        private readonly ParallelOptions _parallelOptions;

        public FasterStorage(
            IFasterStorageOptions storageOptions,
            ITopicEventSender eventSender,
            MetricsCollector metricsCollector)
        {

            if (storageOptions is FasterServerOptions fasterServerOptions)
            {
                _blockStore = new BlockStoreServer(fasterServerOptions, eventSender, metricsCollector);
                _transactionStore = new TransactionStoreServer(fasterServerOptions, eventSender, metricsCollector);
                _actionTraceStore = new ActionTraceStoreServer(fasterServerOptions, eventSender, metricsCollector);
                _abiStore = new AbiStoreServer(fasterServerOptions, eventSender, metricsCollector);

                _transactionStoreClient = new TransactionStoreClient(new FasterClientOptions()
                {
                    IpAddress = fasterServerOptions.IpAddress,
                    Port = fasterServerOptions.TransactionStorePort
                }, eventSender, metricsCollector);
            }
            else if (storageOptions is FasterStandaloneOptions fasterStandaloneOptions)
            {
                _blockStore = new BlockStore(fasterStandaloneOptions, eventSender, metricsCollector);
                _transactionStore = new TransactionStore(fasterStandaloneOptions, eventSender, metricsCollector);
                _actionTraceStore = new ActionTraceStore(fasterStandaloneOptions, eventSender, metricsCollector);
                _abiStore = new AbiStore(fasterStandaloneOptions, eventSender, metricsCollector);
            }
            else if (storageOptions is FasterClientOptions fasterClientOptions)
            {
                _blockStore = new BlockStoreClient(fasterClientOptions, eventSender, metricsCollector);
                _transactionStore = new TransactionStoreClient(fasterClientOptions, eventSender, metricsCollector);
                _actionTraceStore = new ActionTraceStoreClient(fasterClientOptions, eventSender, metricsCollector);
                _abiStore = new AbiStoreClient(fasterClientOptions, eventSender, metricsCollector);
            }

            _parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 6 // TODO, put in config with reasonable name
            };
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
            await _transactionStore.WriteTransaction(transactionTrace);
        }

        public async Task StoreActionTraceAsync(ActionTrace actionTrace)
        {
            await _actionTraceStore.WriteActionTrace(actionTrace);
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
                            await _transactionStore.TryGetTransactionTraceById(transactionId);
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
                                await _actionTraceStore.TryGetActionTraceById(actionTraceId);
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