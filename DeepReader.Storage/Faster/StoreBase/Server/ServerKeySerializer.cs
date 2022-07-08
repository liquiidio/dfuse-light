using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Interfaces;
using FASTER.common;
using BinaryReader = DeepReader.Types.Infrastructure.BinaryReaders.BinaryReader;

namespace DeepReader.Storage.Faster.StoreBase.Server
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
            var reader = new BinaryReader(ref src, ushort.MaxValue);
            _key = TKey.DeserializeKey(reader);
            return ref _key;
        }
    }
}
