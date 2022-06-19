using DeepReader.Storage.Faster.Blocks.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.client;

namespace DeepReader.Storage.Faster.Blocks.Client
{
    internal class BlockClientFunctions : ICallbackFunctions<long, Block, BlockInput, BlockOutput, BlockContext>
    {
        public void DeleteCompletionCallback(ref long key, BlockContext ctx)
        {
            
        }

        public void PublishCompletionCallback(ref long key, ref Block value, BlockContext ctx)
        {
            
        }

        public void ReadCompletionCallback(ref long key, ref BlockInput input, ref BlockOutput output, BlockContext ctx, Status status)
        {
            
        }

        public void RMWCompletionCallback(ref long key, ref BlockInput input, ref BlockOutput output, BlockContext ctx, Status status)
        {
            
        }

        public void SubscribeCallback(ref long key, ref Block value, BlockContext ctx)
        {
            
        }

        public void SubscribeKVCallback(ref long key, ref BlockInput input, ref BlockOutput output, BlockContext ctx, Status status)
        {
            
        }

        public void UpsertCompletionCallback(ref long key, ref Block value, BlockContext ctx)
        {
            
        }
    }
}