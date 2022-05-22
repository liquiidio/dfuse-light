using DeepReader.Storage;
using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.Queries
{
    internal class BlockQuery
    {
        public async Task<Block?> GetBlock(uint block_num, [Service]IStorageAdapter storage)
        {
            var (found, block) = await storage.GetBlockAsync(block_num);
            if (found)
                return block;
            return null;
        }

        public async Task<Block?> GetBlockWithTraces(uint block_num, [Service]IStorageAdapter storage)
        {
            var (found, block) = await storage.GetBlockAsync(block_num, true);
            if (found)
                return block;
            return null;
        }
    }
}