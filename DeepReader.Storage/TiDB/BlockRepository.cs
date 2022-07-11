﻿using DeepReader.Types.StorageTypes;
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

            //if (includeTransactionTraces && !includeActionTraces)
            //{
            //    // Return block with transactiontraces
            //    block = await context.Blocks.Where(b => b.Number == blockNum).Include(b => b.Transactions).FirstOrDefaultAsync();

            //    if (block is null)
            //        return (false, null!);

            //    return (true, block);
            //}
            //else if (includeTransactionTraces && includeActionTraces)
            //{
            //    // Return block with transaction traces and actiontraces
            //    block = await context.Blocks.Where(b => b.Number == blockNum).Include(b => b.Transactions).FirstAsync();
            //}
            //else
            //{
            //    //return just the block;
            //}

            //block = await context.Blocks.Where(b => b.Number == blockNum).Include(b => b.Transactions).FirstOrDefaultAsync();
            block = await context.Blocks.FirstOrDefaultAsync(b => b.Number == blockNum);

            if (block is null)
                return (false, null!);

            return (true, block);
        }
    }
}