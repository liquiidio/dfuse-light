using DeepReader.Storage.Faster.Stores.Abis.Standalone;
using FASTER.common;
using FASTER.core;
using FASTER.server;

namespace DeepReader.Storage.Faster.Stores.Abis.Server
{
    internal sealed class AbiFasterKvProvider : FasterKVProviderBase<ulong, AbiCacheItem, AbiInput,
    AbiOutput, AbiServerFunctions, AbiServerSerializer>
    {
        public AbiFasterKvProvider(FasterKV<ulong, AbiCacheItem> store, AbiServerSerializer serializer,
       SubscribeKVBroker<ulong, AbiCacheItem, AbiInput, IKeyInputSerializer<ulong, AbiInput>> kvBroker = null,
       SubscribeBroker<ulong, AbiCacheItem, IKeySerializer<ulong>> broker = null, bool recoverStore = false,
       MaxSizeSettings maxSizeSettings = null)
       : base(store, serializer, kvBroker, broker, recoverStore, maxSizeSettings)
        {
        }
        public override AbiServerFunctions GetFunctions() => new();
    }
}