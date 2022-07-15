using DeepReader.Types.StorageTypes;
using Microsoft.EntityFrameworkCore;

namespace DeepReader.Storage.TiDB
{
    internal class BlockRepository
    {
        private readonly IDbContextFactory<DataContext> _dbContextFactory;

        public BlockRepository(IDbContextFactory<DataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task WriteBlock(Block block, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            if (context is not null)
            {
                await context.Blocks.AddAsync(block);
                await context.SaveChangesAsync();
            }
        }

        public async Task<(bool, Block)> TryGetBlockById(uint blockNum, bool includeTransactionTraces = false, bool includeActionTraces = false, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (context is null)
                return (false, null!);

            Block? block = null;

            if (includeTransactionTraces && !includeActionTraces)
            {
                block = await context.Blocks
                    .Include(b => b.Transactions)
                    .FirstOrDefaultAsync(b => b.Number == blockNum);
            }
            else if (includeTransactionTraces && includeActionTraces)
            {
                block = await context.Blocks
                    .Include(b => b.Transactions)
                    .ThenInclude(t => t.ActionTraces)
                    .FirstOrDefaultAsync(b => b.Number == blockNum);
            }
            else
            {
                block = await context.Blocks
                    .FirstOrDefaultAsync(b => b.Number == blockNum);
            }

            if (block is null)
                return (false, null!);

            return (true, block);
        }
    }
}