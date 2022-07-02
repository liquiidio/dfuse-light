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
    public class ServerKeySerializer<TKey> : IKeySerializer<TKey>
        where TKey : IKey<TKey>
    {
        [ThreadStatic] 
        static TKey _key;

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

        public unsafe ref TKey ReadKeyByRef(ref byte* src)
        {
            var reader = new UnsafeBinaryUnmanagedReader(src, ushort.MaxValue);
            _key = TKey.DeserializeKey(reader);
            return ref _key;
        }
    }
}
