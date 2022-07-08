using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Interfaces;
using FASTER.common;
using BinaryReader = DeepReader.Types.Infrastructure.BinaryReaders.BinaryReader;

namespace DeepReader.Storage.Faster.StoreBase.Server
{
    public class ServerKeyInputSerializer<TKey, TKKey, TInput> : IKeyInputSerializer<TKKey, TInput>
        where TKey : IKey<TKKey>
        where TInput : IFasterSerializable<TInput>
    {
        [ThreadStatic] 
        private static TKKey _key;

        [ThreadStatic] 
        private static TInput _input;

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

        public unsafe ref TInput ReadInputByRef(ref byte* src)
        {
            _input = TInput.ReadFromFaster(new BinaryReader(ref src, ushort.MaxValue));
            return ref _input;
        }

        public unsafe ref TKKey ReadKeyByRef(ref byte* src)
        {
            _key = TKey.DeserializeKey(new BinaryReader(ref src, ushort.MaxValue));
            return ref _key;
        }
    }
}
