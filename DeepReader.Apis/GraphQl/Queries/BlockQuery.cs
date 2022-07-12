using DeepReader.Apis.GraphQl.DataLoaders.FasterDataLoaders;
using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.Queries
{
    internal class BlockQuery
    {
        public Task<Block> GetBlock(uint block_num, BlockByIdDataLoader dataLoader, CancellationToken cancellationToken)
        {
            return dataLoader.LoadAsync(block_num, cancellationToken);
        }

        public Task<Block> GetBlockWithTraces(uint block_num, BlocksWithTracesByIdDataLoader dataLoader, CancellationToken cancellationToken)
        {
            return dataLoader.LoadAsync(block_num, cancellationToken);
        }
    }
}