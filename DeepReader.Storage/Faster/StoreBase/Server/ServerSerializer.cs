﻿using System.Runtime.CompilerServices;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;
using FASTER.common;

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
            var reader = new UnsafeBinaryUnmanagedReader(dcurr, length);
            _output = TValue.ReadFromFaster(reader);
            return ref _output;
        }

        public int GetLength(ref TValue o)
        {
            var writer = (IBufferWriter)new BinaryWriter(new MemoryStream());
            o.WriteToFaster(writer);
            return (int)writer.Position;
        }

        public unsafe ref TValue ReadInputByRef(ref byte* src)
        {
            var reader = new UnsafeBinaryUnmanagedReader(src, ushort.MaxValue);
            _input = TValue.ReadFromFaster(reader);
            return ref _input;
        }


        public unsafe ref TKKey ReadKeyByRef(ref byte* src)
        {
            var reader = new UnsafeBinaryUnmanagedReader(src, ushort.MaxValue);
            var length = reader.ReadInt32();
            // TODO , we could verify the size here
            _key = TKey.DeserializeKey(reader);
            return ref _key;
        }

        public unsafe ref TValue ReadValueByRef(ref byte* src)
        {
            // this is read from the store so it doesn't have the length-prefix 
            var reader = new UnsafeBinaryUnmanagedReader(src, ushort.MaxValue);
            var length = reader.ReadInt32();
            _value = TValue.ReadFromFaster(reader);
            return ref _value;
        }

        // not sure, seems to just set the src-pointer to something?!
        public unsafe void SkipOutput(ref byte* dcurr)
        {
            // TODO, not sure if this has length-prefix or not
            var length = Unsafe.Read<int>(dcurr); // read length of TransactionTrace in bytes
            dcurr += sizeof(int); // increase pointer by size of long
            var reader = new UnsafeBinaryUnmanagedReader(dcurr, length);
            TValue.ReadFromFaster(reader);
            dcurr += length;
        }

        public unsafe bool Write(ref TKKey k, ref byte* dst, int length) // length is max memory available/left
        {
            var writer = (IBufferWriter)new BinaryWriter(new UnmanagedMemoryStream(dst, length));
            TKey.SerializeKey(k, writer); // serialize !
            dst += writer.Position;

            return true;
        }

        public unsafe bool Write(ref TValue v, ref byte* dst, int length) // length is max memory available/left
        {
            var writer = (IBufferWriter)new BinaryWriter(new UnmanagedMemoryStream(dst, length));
            writer.Position += sizeof(int); // reserve sizeof(long) so we can write the length after serialization
            v.WriteToFaster(writer); // serialize !
            Unsafe.Write(dst, writer.Position - sizeof(int)); // write length into reserved memory
            dst += writer.Position; // set pointer to new end

            return true;
        }
    }
}
