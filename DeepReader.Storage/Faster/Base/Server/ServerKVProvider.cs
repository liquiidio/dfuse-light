﻿using System;
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
    public class ServerKVProvider<TKey, TKKey, TValue> : FasterKVProviderBase<TKKey, TValue, TKKey,
        TValue, ServerFunctions<TKKey, TValue>, ServerSerializer<TKey, TKKey, TValue>>
    where TKey : IKey<TKKey>
    where TValue : IFasterSerializable<TValue>
    {
        public ServerKVProvider(FasterKV<TKKey, TValue> store, ServerSerializer<TKey, TKKey, TValue> serializer,
            SubscribeKVBroker<TKKey, TValue, TKKey, IKeyInputSerializer<TKKey, TKKey>> kvBroker = null,
            SubscribeBroker<TKKey, TValue, IKeySerializer<TKKey>> broker = null, bool recoverStore = false,
            MaxSizeSettings maxSizeSettings = null)
            : base(store, serializer, kvBroker, broker, recoverStore, maxSizeSettings)
        {

        }

        public override ServerFunctions<TKKey, TValue> GetFunctions() => new();
    }
}
