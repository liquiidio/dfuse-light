using DeepReader.Storage.Faster.Blocks;
using DeepReader.Storage.Faster.Transactions;
using DeepReader.Types.FlattenedTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster
{
    internal class FasterStorage : IStorageAdapter
    {
        private readonly BlockStore _blockStore;
        private readonly TransactionStore _transactionStore;

        public FasterStorage()
        {
            _blockStore = new BlockStore();
            _transactionStore = new TransactionStore();
        }

        public async Task StoreBlockAsync(FlattenedBlock block) // compress, store, index
        {
            await _blockStore.WriteBlock(block);
        }

        public async Task StoreTransactionAsync(FlattenedTransactionTrace transactionTrace)  // compress, store, index
        {
            await _transactionStore.WriteTransaction(transactionTrace);
        }
    }
}
