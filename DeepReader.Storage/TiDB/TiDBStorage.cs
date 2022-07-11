using DeepReader.Storage.Faster.Abis;
using DeepReader.Types.EosTypes;
using DeepReader.Types.StorageTypes;
using System.Reflection;

namespace DeepReader.Storage.TiDB
{
    internal class TiDBStorage : IStorageAdapter
    {
        private readonly BlockRepository _blockRepository;
        private readonly TransactionRepository _transactionRepository;
        private readonly ActionTraceRepository _actionTraceRepository;

        public TiDBStorage(BlockRepository blockRepository, TransactionRepository transactionRepository, ActionTraceRepository actionTraceRepository)
        {
            _blockRepository = blockRepository;
            _transactionRepository = transactionRepository;
            _actionTraceRepository = actionTraceRepository;
        }

        public long BlocksIndexed => 0;

        public long TransactionsIndexed => 0;

        public async Task<(bool, ActionTrace)> GetActionTraceAsync(ulong globalSequence)
        {
            return await _actionTraceRepository.TryGetActionTraceById(globalSequence);
        }

        public async Task<(bool, Block)> GetBlockAsync(uint blockNum, bool includeTransactionTraces = false, bool includeActionTraces = false)
        {
            return await _blockRepository.TryGetBlockById(blockNum, includeTransactionTraces, includeActionTraces);
        }

        public async Task<(bool, TransactionTrace)> GetTransactionAsync(string transactionId, bool includeActionTraces = false)
        {
            // Todo
            return await _transactionRepository.TryGetTransactionTraceById(transactionId);
        }

        public async Task StoreActionTraceAsync(ActionTrace actionTrace)
        {
            await _actionTraceRepository.WriteActionTrace(actionTrace);
        }

        public async Task StoreBlockAsync(Block block)
        {
            Serilog.Log.Information("tidb_store_block_async called.");
            await _blockRepository.WriteBlock(block);
        }

        public async Task StoreTransactionAsync(TransactionTrace transactionTrace)
        {
            await _transactionRepository.WriteTransaction(transactionTrace);
        }

        public Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account)
        {
            throw new NotImplementedException();
        }

        public Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly)
        {
            throw new NotImplementedException();
        }
    }
}
