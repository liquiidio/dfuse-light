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
    internal class KeyValueClientFunctions<TKey, TValue> : ICallbackFunctions<TKey, TValue, TValue, TValue, KeyValueContext>
    where TKey : IKey<TKey>
    where TValue : IEosioSerializable<TValue>, IFasterSerializable<TValue>
    {
        public void ReadCompletionCallback(ref TKey key, ref TValue input, ref TValue output, KeyValueContext ctx, Status status)
        {
            Debug.WriteLine("ReadCompletionCallback");
        }

        public void UpsertCompletionCallback(ref TKey key, ref TValue value, KeyValueContext ctx)
        {
            Debug.WriteLine("UpsertCompletionCallback");

        }

        public void RMWCompletionCallback(ref TKey key, ref TValue input, ref TValue output, KeyValueContext ctx, Status status)
        {
            Debug.WriteLine("RMWCompletionCallback");
        }

        public void DeleteCompletionCallback(ref TKey key, KeyValueContext ctx)
        {
            Debug.WriteLine("DeleteCompletionCallback");
        }

        public void SubscribeKVCallback(ref TKey key, ref TValue input, ref TValue output, KeyValueContext ctx, Status status)
        {
            Debug.WriteLine("SubscribeKVCallback");
        }

        public void PublishCompletionCallback(ref TKey key, ref TValue value, KeyValueContext ctx)
        {
            Debug.WriteLine("PublishCompletionCallback");
        }

        public void SubscribeCallback(ref TKey key, ref TValue value, KeyValueContext ctx)
        {
            Debug.WriteLine("SubscribeCallback");
        }
    }
}
