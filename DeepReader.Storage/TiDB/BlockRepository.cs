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

        public async Task<(bool, Block)> TryGetBlockById(uint blockNum, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (context is not null)
            {
                var block = await context.Blocks.FirstOrDefaultAsync(b => b.Number == blockNum);

                if (block is not null)
                    return (true, block);
            }
            return (false, null!);
        }
    }
}