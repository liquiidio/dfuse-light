using DeepReader.Storage;
using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.DataLoaders.FasterDataLoaders
{
    internal class BlocksWithTracesByIdDataLoader : BatchDataLoader<uint, Block>
    {
        private readonly IStorageAdapter _storageAdapter;

        public BlocksWithTracesByIdDataLoader(IBatchScheduler batchScheduler, IStorageAdapter storageAdapter) : base(batchScheduler)
        {
            _storageAdapter = storageAdapter;
        }

        protected override async Task<IReadOnlyDictionary<uint, Block>> LoadBatchAsync(IReadOnlyList<uint> keys, CancellationToken cancellationToken)
        {
            //Serilog.Log.Information($"{nameof(BlocksWithTracesByIdDataLoader)} called.");
            var list = new List<Block>();
            foreach (var key in keys)
            {
                var (found, block) = await _storageAdapter.GetBlockAsync(key, true);
                if (found)
                    list.Add(block);
            }
            return list.ToDictionary(b => b.Number);
        }
    }
}