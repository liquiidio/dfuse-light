using DeepReader.Storage.Faster.Abis.Standalone;
using FASTER.common;
using FASTER.core;
using FASTER.server;

namespace DeepReader.Storage.Faster.Abis.Server
{
    internal sealed class AbiFasterKVProvider : FasterKVProviderBase<ulong, AbiCacheItem, AbiInput,
    AbiOutput, AbiServerFunctions, AbiServerSerializer>
    {
        public AbiFasterKVProvider(FasterKV<ulong, AbiCacheItem> store, AbiServerSerializer serializer,
       SubscribeKVBroker<ulong, AbiCacheItem, AbiInput, IKeyInputSerializer<ulong, AbiInput>> kvBroker = null,
       SubscribeBroker<ulong, AbiCacheItem, IKeySerializer<ulong>> broker = null, bool recoverStore = false,
       MaxSizeSettings maxSizeSettings = null)
       : base(store, serializer, kvBroker, broker, recoverStore, maxSizeSettings)
        {
        }
        public override AbiServerFunctions GetFunctions() => new();
    }
}