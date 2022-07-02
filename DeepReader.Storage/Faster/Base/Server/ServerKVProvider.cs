using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.Interfaces;
using FASTER.common;
using FASTER.core;
using FASTER.server;

namespace DeepReader.Storage.Faster.Test.Server
{
    public class ServerKVProvider<TKey, TValue> : FasterKVProviderBase<TKey, TValue, TKey,
        TValue, ServerFunctions<TKey, TValue>, ServerSerializer<TKey, TValue>>
    where TKey : IKey<TKey>
    where TValue : IEosioSerializable<TValue>, IFasterSerializable<TValue>
    {
        public ServerKVProvider(FasterKV<TKey, TValue> store, ServerSerializer<TKey, TValue> serializer,
            SubscribeKVBroker<TKey, TValue, TKey, IKeyInputSerializer<TKey, TKey>> kvBroker = null,
            SubscribeBroker<TKey, TValue, IKeySerializer<TKey>> broker = null, bool recoverStore = false,
            MaxSizeSettings maxSizeSettings = null)
            : base(store, serializer, kvBroker, broker, recoverStore, maxSizeSettings)
        {

        }

        public override ServerFunctions<TKey, TValue> GetFunctions() => new();
    }
}
