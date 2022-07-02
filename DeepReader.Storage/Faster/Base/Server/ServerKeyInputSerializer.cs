using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Interfaces;
using FASTER.common;

namespace DeepReader.Storage.Faster.Test.Server
{
    public class ServerKeyInputSerializer<TKey, TKKey> : IKeyInputSerializer<TKKey, TKKey>
        where TKey : IKey<TKKey>
    {
        [ThreadStatic] 
        private static TKKey _key;

        [ThreadStatic] 
        private static TKKey _input;

        public bool Match(ref TKKey k, bool asciiKey, ref TKKey pattern, bool asciiPattern)
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

        public unsafe ref TKKey ReadInputByRef(ref byte* src)
        {
            _input = TKey.DeserializeKey(new UnsafeBinaryUnmanagedReader(ref src, ushort.MaxValue));
            return ref _input;
        }

        public unsafe ref TKKey ReadKeyByRef(ref byte* src)
        {
            _key = TKey.DeserializeKey(new UnsafeBinaryUnmanagedReader(ref src, ushort.MaxValue));
            return ref _key;
        }
    }
}
