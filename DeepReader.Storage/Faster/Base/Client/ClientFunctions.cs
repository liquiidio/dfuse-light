using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.Interfaces;
using FASTER.client;

namespace DeepReader.Storage.Faster.Test.Client
{
    internal class ClientFunctions<TKey, TKKey, TValue> : ICallbackFunctions<TKKey, TValue, TValue, TValue, KeyValueContext>
    where TKey : IKey<TKKey>
    where TValue : IFasterSerializable<TValue>
    {
        public void ReadCompletionCallback(ref TKKey key, ref TValue input, ref TValue output, KeyValueContext ctx, Status status)
        {
            Debug.WriteLine("ReadCompletionCallback");
        }

        public void UpsertCompletionCallback(ref TKKey key, ref TValue value, KeyValueContext ctx)
        {
            Debug.WriteLine("UpsertCompletionCallback");

        }

        public void RMWCompletionCallback(ref TKKey key, ref TValue input, ref TValue output, KeyValueContext ctx, Status status)
        {
            Debug.WriteLine("RMWCompletionCallback");
        }

        public void DeleteCompletionCallback(ref TKKey key, KeyValueContext ctx)
        {
            Debug.WriteLine("DeleteCompletionCallback");
        }

        public void SubscribeKVCallback(ref TKKey key, ref TValue input, ref TValue output, KeyValueContext ctx, Status status)
        {
            Debug.WriteLine("SubscribeKVCallback");
        }

        public void PublishCompletionCallback(ref TKKey key, ref TValue value, KeyValueContext ctx)
        {
            Debug.WriteLine("PublishCompletionCallback");
        }

        public void SubscribeCallback(ref TKKey key, ref TValue value, KeyValueContext ctx)
        {
            Debug.WriteLine("SubscribeCallback");
        }
    }
}
