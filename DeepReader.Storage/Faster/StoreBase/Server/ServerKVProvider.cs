using DeepReader.Types.Interfaces;
using FASTER.common;
using FASTER.core;
using FASTER.server;

namespace DeepReader.Storage.Faster.StoreBase.Server
{
    public class ServerKvProvider<TKey, TKKey, TValue> : FasterKVProviderBase<TKKey, TValue, TKKey,
        TValue, ServerFunctions<TKKey, TValue>, ServerSerializer<TKey, TKKey, TValue>>
    where TKey : IKey<TKKey>
    where TValue : IFasterSerializable<TValue>
    {
        public ServerKvProvider(FasterKV<TKKey, TValue> store, ServerSerializer<TKey, TKKey, TValue> serializer,
            SubscribeKVBroker<TKKey, TValue, TKKey, IKeyInputSerializer<TKKey, TKKey>> kvBroker = null,
            SubscribeBroker<TKKey, TValue, IKeySerializer<TKKey>> broker = null, bool recoverStore = false,
            MaxSizeSettings maxSizeSettings = null)
            : base(store, serializer, kvBroker, broker, recoverStore, maxSizeSettings)
        {

        }

        public override ServerFunctions<TKKey, TValue> GetFunctions() => new();
    }
}
