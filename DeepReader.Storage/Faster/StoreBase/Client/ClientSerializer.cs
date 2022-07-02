using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;
using FASTER.common;

namespace DeepReader.Storage.Faster.Test
{
    namespace DeepReader.Storage.Faster.Transactions.Client
    {
        public class ClientSerializer<TKey, TKKey, TValue> : IClientSerializer<TKKey, TValue, TValue, TValue>
            where TKey : IKey<TKKey>
            where TValue : IFasterSerializable<TValue>
        {

            public unsafe bool Write(ref TKKey k, ref byte* dst, int length)
            {
                ReserveHeader(ref dst);
                var writer = new UnsafeBinaryUnmanagedWriter(new UnmanagedMemoryStream(dst, length - sizeof(int), length - sizeof(int), FileAccess.Write));
                TKey.SerializeKey(k, writer);
                SetHeader(ref dst, writer);
                SetPos(ref dst, writer);
                return true;
            }

            public unsafe bool Write(ref TValue v, ref byte* dst, int length)
            {
                ReserveHeader(ref dst);
                var writer = new UnsafeBinaryUnmanagedWriter(new UnmanagedMemoryStream(dst, length - sizeof(int), length - sizeof(int), FileAccess.Write));
                v.WriteToFaster(writer);
                SetHeader(ref dst, writer);
                SetPos(ref dst, writer);
                return true;
            }

            public unsafe TValue ReadOutput(ref byte* src)
            {
                var length = Unsafe.Read<int>(src);
                src += sizeof(int);
                var reader = new UnsafeBinaryUnmanagedReader(ref src, length);
                return TValue.ReadFromFaster(reader);
            }

            public unsafe TKKey ReadKey(ref byte* src)
            {
                return TKey.DeserializeKey(new UnsafeBinaryUnmanagedReader(src, ushort.MaxValue));
            }

            public unsafe TValue ReadValue(ref byte* src)
            {
                var length = Unsafe.Read<int>(src);
                src += sizeof(int);
                var reader = new UnsafeBinaryUnmanagedReader(src, length);
                return TValue.ReadFromFaster(reader);
            }

            private unsafe void ReserveHeader(ref byte* dst)
            {
                dst += sizeof(int);
            }

            // TODO, are these Masks really needed?
            const int KHeaderMask = 0x3 << 30;
            const int KUnserializedBitMask = 1 << 31;

            private unsafe void SetHeader(ref byte* dst, UnsafeBinaryUnmanagedWriter writer)
            {
                dst -= sizeof(int); // we go back 4 bytes
                var length = (int)writer.Position;
                *(int*)dst = (length | KUnserializedBitMask) & ~KHeaderMask;
                dst += sizeof(int); // we go forward 4 bytes
            }

            private unsafe void SetPos(ref byte* dst, UnsafeBinaryUnmanagedWriter writer)
            {
                dst += writer.Position;
            }

            private unsafe void ReadHeader(ref byte* dst, UnsafeBinaryUnmanagedWriter writer)
            {
                var length = (int)writer.Position;
                *(int*)dst = (length & KHeaderMask);
            }
        }
    }
}
