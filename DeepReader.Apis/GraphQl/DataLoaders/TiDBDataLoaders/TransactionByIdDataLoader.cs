using DeepReader.Storage.TiDB;
using DeepReader.Types.StorageTypes;
using Microsoft.EntityFrameworkCore;

namespace DeepReader.Apis.GraphQl.DataLoaders.TiDBDataLoaders
{
    internal class TransactionByIdDataLoader : BatchDataLoader<string, TransactionTrace>
    {
        private readonly IDbContextFactory<DataContext> _dbContextFactory;

        public TransactionByIdDataLoader(IBatchScheduler batchScheduler, IDbContextFactory<DataContext> dbContextFactory) : base(batchScheduler)
        {
            _dbContextFactory = dbContextFactory;
        }

        protected override async Task<IReadOnlyDictionary<string, TransactionTrace>> LoadBatchAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            return await context.TransactionTraces.Where(t => keys.Contains(t.Id.StringVal)).ToDictionaryAsync(t => t.Id.StringVal, cancellationToken);
        }
    }
}