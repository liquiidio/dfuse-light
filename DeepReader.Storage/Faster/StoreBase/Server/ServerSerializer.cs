using System.Diagnostics;
using System.Runtime.CompilerServices;
using DeepReader.Types.Helpers;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;
using FASTER.common;
using BinaryReader = DeepReader.Types.Infrastructure.BinaryReaders.BinaryReader;
using BinaryWriter = DeepReader.Types.Infrastructure.BinaryWriters.BinaryWriter;

namespace DeepReader.Storage.Faster.StoreBase.Server
{
    public class ServerSerializer<TKey, TKKey, TValue> : IServerSerializer<TKKey, TValue, TValue, TValue>
        where TKey : IKey<TKKey>
        where TValue : IFasterSerializable<TValue>
    {
        [ThreadStatic] private static TValue _input;

        [ThreadStatic] private static TValue _output;

        [ThreadStatic] private static TKKey _key;

        [ThreadStatic] private static TValue _value;

        public unsafe ref TValue AsRefOutput(byte* dcurr, int length)
        {
            // TODO, not sure if this has length-prefix or not
            // this is read from the store so it doesn't have the length-prefix 
            var reader = new BinaryReader(ref dcurr, length);
            _output = TValue.ReadFromFaster(reader);
            var test = _output;
            return ref _output;
        }

        public int GetLength(ref TValue o)
        {
            var writer = new Types.Infrastructure.BinaryWriters.BinaryWriter(new System.IO.BinaryWriter(new MemoryStream()));
            o.WriteToFaster(writer);

            return (int)writer.Position;
        }

        public unsafe ref TValue ReadInputByRef(ref byte* src)
        {
            var reader = new BinaryReader(ref src, ushort.MaxValue);
            var length = reader.ReadInt32();
            if (length != 0)
            {
                _input = TValue.ReadFromFaster(reader);
                var test = _input;
                src += reader.Position;
            }
            else
                _input = default(TValue);
            return ref _input;
        }


        public unsafe ref TKKey ReadKeyByRef(ref byte* src)
        {
            var reader = new BinaryReader(ref src, ushort.MaxValue);
            var length = reader.ReadInt32();
            // TODO , we could verify the size here
            _key = TKey.DeserializeKey(reader);
            var test = _key;
            src += reader.Position;
            return ref _key;
        }

        public unsafe ref TValue ReadValueByRef(ref byte* src)
        {
            int length;
            try
            {
                // this is read from the store so it doesn't have the length-prefix 
                var reader = new BinaryReader(ref src, ushort.MaxValue);
                length = reader.ReadInt32();
                _value = TValue.ReadFromFaster(reader);
                var test = _value;
                src += reader.Position;
                return ref _value;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            _value = default(TValue);
            return ref _value;
        }

        // not sure, seems to just set the src-pointer to something?!
        public unsafe void SkipOutput(ref byte* dcurr)
        {
            // TODO, not sure if this has length-prefix or not
            var length = Unsafe.Read<int>(dcurr); // read length of TransactionTrace in bytes
            dcurr += sizeof(int); // increase pointer by size of long
            var reader = new BinaryReader(ref dcurr, length);
            TValue.ReadFromFaster(reader);
            dcurr += length;
        }

        public unsafe bool Write(ref TKKey k, ref byte* dst, int length) // length is max memory available/left
        {
            var writer = new BinaryWriter(dst, length);
            TKey.SerializeKey(k, writer); // serialize !
            dst += writer.Position;

            return true;
        }

        public unsafe bool Write(ref TValue v, ref byte* dst, int length) // length is max memory available/left
        {
            var writer = new BinaryWriter(dst, length);
            writer.Position += sizeof(int); // reserve sizeof(long) so we can write the length after serialization
            v.WriteToFaster(writer); // serialize !
            Unsafe.Write(dst, writer.Position - sizeof(int)); // write length into reserved memory
            dst += writer.Position; // set pointer to new end

            return true;
        }
    }
}
