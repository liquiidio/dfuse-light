using DeepReader.Storage.Faster.StoreBase.Server;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;
using FASTER.server;

namespace DeepReader.Storage.Faster.Stores.Abis.Custom
{
    public class ServerAbiKVProvider : FasterKVProviderBase<ulong, AbiCacheItem, AbiCacheItem,
        AbiCacheItem, ServerAbiFunctions, ServerSerializer<UlongKey, ulong, AbiCacheItem>>
    {
        public ServerAbiKVProvider(FasterKV<ulong, AbiCacheItem> store, ServerSerializer<UlongKey, ulong, AbiCacheItem> serializer,
            SubscribeKVBroker<ulong, AbiCacheItem, AbiCacheItem, IKeyInputSerializer<ulong, AbiCacheItem>> kvBroker = null,
            SubscribeBroker<ulong, AbiCacheItem, IKeySerializer<ulong>> broker = null, bool recoverStore = false,
            MaxSizeSettings maxSizeSettings = null)
            : base(store, serializer, kvBroker, broker, recoverStore, maxSizeSettings)
        {

        }

        public override ServerAbiFunctions GetFunctions() => new();
    }
}
