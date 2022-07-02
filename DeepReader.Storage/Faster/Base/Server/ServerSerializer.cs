using System.Runtime.CompilerServices;
using DeepReader.Types.Infrastructure;
using DeepReader.Types.Interfaces;
using FASTER.common;

namespace DeepReader.Storage.Faster.Test.Server
{
    public class ServerSerializer<TKey, TValue> : IServerSerializer<TKey, TValue, TKey, TValue>
        where TKey : IKey<TKey>
        where TValue : IEosioSerializable<TValue>, IFasterSerializable<TValue>
    {
        [ThreadStatic] private static TKey _input;

        [ThreadStatic] private static TValue _output;

        [ThreadStatic] private static TKey _key;

        [ThreadStatic] private static TValue _value;

        public unsafe ref TValue AsRefOutput(byte* dcurr, int length)
        {
            // TODO, not sure if this has length-prefix or not
            // this is read from the store so it doesn't have the length-prefix 
            var reader = new UnsafeBinaryUnmanagedReader(dcurr, length);
            _output = TValue.ReadFromBinaryReader(reader);
            return ref _output;
        }

        public int GetLength(ref TValue o)
        {
            var writer = new BinaryWriter(new MemoryStream());
            o.WriteToFaster(writer);
            return (int)writer.BaseStream.Position;
        }

        public unsafe ref TKey ReadInputByRef(ref byte* src)
        {
            var reader = new UnsafeBinaryUnmanagedReader(src, ushort.MaxValue);
            _input = TKey.DeserializeKey(reader);
            return ref _input;
        }


        public unsafe ref TKey ReadKeyByRef(ref byte* src)
        {
            var reader = new UnsafeBinaryUnmanagedReader(src, ushort.MaxValue);
            var length = reader.ReadInt32();
            // Todo, we could verify the size here
            _key = TKey.DeserializeKey(reader);
            return ref _key;
        }

        public unsafe ref TValue ReadValueByRef(ref byte* src)
        {
            // this is read from the store so it doesn't have the length-prefix 
            var reader = new UnsafeBinaryUnmanagedReader(src, ushort.MaxValue);
            var length = reader.ReadInt32();
            _value = TValue.ReadFromBinaryReader(reader);
            return ref _value;
        }

        // not sure, seems to just set the src-pointer to something?!
        public unsafe void SkipOutput(ref byte* dcurr)
        {
            // TODO, not sure if this has length-prefix or not
            var length = Unsafe.Read<int>(dcurr); // read length of TransactionTrace in bytes
            dcurr += sizeof(int); // increase pointer by size of long
            var reader = new UnsafeBinaryUnmanagedReader(dcurr, length);
            TValue.ReadFromBinaryReader(reader);
            dcurr += length;
        }

        public unsafe bool Write(ref TKey k, ref byte* dst, int length) // length is max memory available/left
        {
            var writer = new BinaryWriter(new UnmanagedMemoryStream(dst, length));
            TKey.SerializeKey(k, writer); // serialize !
            dst += writer.BaseStream.Position;

            return true;
        }

        public unsafe bool Write(ref TValue v, ref byte* dst, int length) // length is max memory available/left
        {
            var writer = new BinaryWriter(new UnmanagedMemoryStream(dst, length));
            writer.BaseStream.Position += sizeof(int); // reserve sizeof(long) so we can write the length after serialization
            v.WriteToFaster(writer); // serialize !
            Unsafe.Write(dst, writer.BaseStream.Position - sizeof(int)); // write length into reserved memory
            dst += writer.BaseStream.Position; // set pointer to new end

            return true;
        }
    }
}
