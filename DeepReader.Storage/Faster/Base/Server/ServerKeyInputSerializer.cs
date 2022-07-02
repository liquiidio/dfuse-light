using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure;
using DeepReader.Types.Interfaces;
using FASTER.common;

namespace DeepReader.Storage.Faster.Test.Server
{
    public class ServerKeyInputSerializer<TKey> : IKeyInputSerializer<TKey, TKey>
        where TKey : IKey<TKey>
    {
        [ThreadStatic] 
        private static TKey _key;

        [ThreadStatic] 
        private static TKey _input;

        public bool Match(ref TKey k, bool asciiKey, ref TKey pattern, bool asciiPattern)
        {
            // I have no idea what needs to be done here ...
            if (k.Equals(pattern))
                return true;
            else
            {
                // TODO
                Console.WriteLine("Match false");
            }

            return true;
        }

        public unsafe ref TKey ReadInputByRef(ref byte* src)
        {
            _input = TKey.DeserializeKey(new UnsafeBinaryUnmanagedReader(ref src, ushort.MaxValue));
            return ref _input;
        }

        public unsafe ref TKey ReadKeyByRef(ref byte* src)
        {
            _key = TKey.DeserializeKey(new UnsafeBinaryUnmanagedReader(ref src, ushort.MaxValue));
            return ref _key;
        }
    }
}
