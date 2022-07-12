using DeepReader.Storage;
using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.DataLoaders.FasterDataLoaders
{
    internal class TransactionByIdDataLoader : BatchDataLoader<string, TransactionTrace>
    {
        private readonly IStorageAdapter _storageAdapter;

        public TransactionByIdDataLoader(IBatchScheduler batchScheduler, IStorageAdapter storageAdapter) : base(batchScheduler)
        {
            _storageAdapter = storageAdapter;
        }

        protected override async Task<IReadOnlyDictionary<string, TransactionTrace>> LoadBatchAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken)
        {
            //Serilog.Log.Information($"{nameof(TransactionByIdDataLoader)} called.");
            var list = new List<TransactionTrace>();
            foreach (var key in keys)
            {
                var (found, transaction) = await _storageAdapter.GetTransactionAsync(key);
                if (found)
                    list.Add(transaction);
            }
            return list.ToDictionary(t => t.Id.ToString());
        }
    }
}