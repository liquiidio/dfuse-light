using DeepReader.Storage.TiDB;
using DeepReader.Types.StorageTypes;
using Microsoft.EntityFrameworkCore;

namespace DeepReader.Apis.GraphQl.DataLoaders.TiDBDataLoaders
{
    internal class BlocksWithTracesByIdDataLoader : BatchDataLoader<uint, Block>
    {
        private readonly IDbContextFactory<DataContext> _dbContextFactory;

        public BlocksWithTracesByIdDataLoader(IBatchScheduler batchScheduler, IDbContextFactory<DataContext> dbContextFactory) : base(batchScheduler)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override async Task<IReadOnlyDictionary<uint, Block>> LoadBatchAsync(IReadOnlyList<uint> keys, CancellationToken cancellationToken)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            return await context.Blocks.Where(b => keys.Contains(b.Number)).ToDictionaryAsync(t => t.Number, cancellationToken);
        }
    }
}