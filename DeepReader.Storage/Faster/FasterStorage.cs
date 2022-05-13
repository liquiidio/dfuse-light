using DeepReader.Storage.Faster.Blocks;
using DeepReader.Storage.Faster.Transactions;
using DeepReader.Storage.Options;
using DeepReader.Types.FlattenedTypes;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.Options;

namespace DeepReader.Storage.Faster
{
    internal class FasterStorage : IStorageAdapter
    {
        private readonly BlockStore _blockStore;
        private readonly TransactionStore _transactionStore;

        private FasterStorageOptions _fasterStorageOptions;

        public FasterStorage(
            IOptionsMonitor<FasterStorageOptions> storageOptionsMonitor,
            ITopicEventSender eventSender,
            MetricsCollector metricsCollector)
        {
            _fasterStorageOptions = storageOptionsMonitor.CurrentValue;
            storageOptionsMonitor.OnChange(OnFasterStorageOptionsChanged);

            _blockStore = new BlockStore(_fasterStorageOptions, eventSender, metricsCollector);
            _transactionStore = new TransactionStore(_fasterStorageOptions, eventSender, metricsCollector);
        }

        private void OnFasterStorageOptionsChanged(FasterStorageOptions newOptions)
        {
            _fasterStorageOptions = newOptions;
        }

        public long BlocksIndexed => _blockStore.BlocksIndexed;

        public long TransactionsIndexed => _transactionStore.TransactionsIndexed;

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
    }
}
