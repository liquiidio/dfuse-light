using DeepReader.Types.StorageTypes;
using Microsoft.EntityFrameworkCore;

namespace DeepReader.Storage.TiDB
{
    internal class BlockRepository
    {
        private readonly DataContext _context;

        public BlockRepository(DataContext context)
        {
            _context = context;
        }

        public async Task WriteBlock(Block block)
        {
            await _context.Blocks.AddAsync(block);
            await _context.SaveChangesAsync();
        }

        public async Task<(bool, Block)> TryGetBlockById(uint blockNum)
        {
            var block = await _context.Blocks.FirstOrDefaultAsync(b => b.Number == blockNum);

            if (block is null)
                return (false, null!);
            return (true, block);
        }
    }
}