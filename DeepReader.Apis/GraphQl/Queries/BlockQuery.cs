using DeepReader.Storage;
using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.Queries
{
    internal class BlockQuery
    {
        public async Task<FlattenedBlock?> GetBlock(uint block_num, [Service]IStorageAdapter storage)
        {
            var (found, block) = await storage.GetBlockAsync(block_num);
            if (found)
                return block;
            return null;
        }

        public async Task<FlattenedBlock?> GetBlockWithTraces(uint block_num, [Service]IStorageAdapter storage)
        {
            var (found, block) = await storage.GetBlockAsync(block_num, true);
            if (found)
                return block;
            return null;
        }
    }
}