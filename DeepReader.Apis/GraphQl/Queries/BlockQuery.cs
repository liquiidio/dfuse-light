using DeepReader.Apis.GraphQl.DataLoaders;
using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.Queries
{
    internal class BlockQuery
    {
        //public async Task<Block?> GetBlock(uint block_num, BlockByIdDataLoader dataLoader, [Service]IStorageAdapter storage)
        public Task<Block> GetBlock(uint block_num, BlockByIdDataLoader dataLoader, CancellationToken cancellationToken)
        {
            //var (found, block) = await storage.GetBlockAsync(block_num);
            //if (found)
            //    return block;
            //return null;
            return dataLoader.LoadAsync(block_num, cancellationToken);
        }

        //public Task<Block?> GetBlockWithTraces(uint block_num, [Service]IStorageAdapter storage)
        public Task<Block> GetBlockWithTraces(uint block_num, BlocksWithTracesByIdDataLoader dataLoader, CancellationToken cancellationToken)
        {
            //var (found, block) = await storage.GetBlockAsync(block_num, true);
            //if (found)
            //    return block;
            //return null;
            return dataLoader.LoadAsync(block_num, cancellationToken);
        }
    }
}