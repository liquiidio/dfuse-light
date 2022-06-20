using DeepReader.Storage.Faster.Abis.Standalone;
using FASTER.client;

namespace DeepReader.Storage.Faster.Abis.Client
{
    internal class AbiClientFunctions : ICallbackFunctions<ulong, AbiCacheItem, AbiInput, AbiOutput, AbiContext>
    {
        public void DeleteCompletionCallback(ref ulong key, AbiContext ctx)
        {
        }

        public void PublishCompletionCallback(ref ulong key, ref AbiCacheItem value, AbiContext ctx)
        {
        }

        public void ReadCompletionCallback(ref ulong key, ref AbiInput input, ref AbiOutput output, AbiContext ctx, Status status)
        {
        }

        public void RMWCompletionCallback(ref ulong key, ref AbiInput input, ref AbiOutput output, AbiContext ctx, Status status)
        {
        }

        public void SubscribeCallback(ref ulong key, ref AbiCacheItem value, AbiContext ctx)
        {
        }

        public void SubscribeKVCallback(ref ulong key, ref AbiInput input, ref AbiOutput output, AbiContext ctx, Status status)
        {
        }

        public void UpsertCompletionCallback(ref ulong key, ref AbiCacheItem value, AbiContext ctx)
        {
        }
    }
}