using DeepReader.Storage.Faster.Blocks;
using DeepReader.Storage.Faster.Transactions;
using DeepReader.Storage.Options;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.FlattenedTypes;
using FASTER.core;
using Microsoft.Extensions.Options;

namespace DeepReader.Storage.Faster
{
    internal class FasterStorage : IStorageAdapter
    {
        private readonly BlockStore _blockStore;
        private readonly TransactionStore _transactionStore;

        private FasterStorageOptions _fasterStorageOptions;

        public FasterStorage(IOptionsMonitor<FasterStorageOptions> storageOptionsMonitor)
        {
            _fasterStorageOptions = storageOptionsMonitor.CurrentValue;
            storageOptionsMonitor.OnChange(OnFasterStorageOptionsChanged);

            _blockStore = new BlockStore();
            _transactionStore = new TransactionStore();
        }

        private void OnFasterStorageOptionsChanged(FasterStorageOptions newOptions)
        {
            _fasterStorageOptions = newOptions;
        }

        public async Task StoreBlockAsync(FlattenedBlock block) // compress, store, index
        {
            await _blockStore.WriteBlock(block);
        }

        public async Task StoreTransactionAsync(FlattenedTransactionTrace transactionTrace)  // compress, store, index
        {
            var status = await _transactionStore.WriteTransaction(transactionTrace);
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
