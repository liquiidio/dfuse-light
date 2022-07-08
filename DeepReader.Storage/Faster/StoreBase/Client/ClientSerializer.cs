using System.Diagnostics;
using System.Runtime.CompilerServices;
using DeepReader.Types.Helpers;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using BinaryReader = DeepReader.Types.Infrastructure.BinaryReaders.BinaryReader;
using BinaryWriter = DeepReader.Types.Infrastructure.BinaryWriters.BinaryWriter;

namespace DeepReader.Storage.Faster.StoreBase.Client
{
    namespace DeepReader.Storage.Faster.Transactions.Client
    {
        public class ClientSerializer<TKey, TKKey, TValue> : IClientSerializer<TKKey, TValue, TValue, TValue>
            where TKey : IKey<TKKey>
            where TValue : IFasterSerializable<TValue>
        {

            public unsafe bool Write(ref TKKey k, ref byte* dst, int length)
            {
                if (length > 5000)
                {
                    ReserveHeader(ref dst);
                    var writer = new BinaryWriter(dst, length);
                    TKey.SerializeKey(k, writer);
                    SetHeader(ref dst, writer);
                    SetPos(ref dst, writer);
                    return true;
                }
                return false;
            }

            public unsafe bool Write(ref TValue v, ref byte* dst, int length)
            {
                if (length > 5000)
                {
                    ReserveHeader(ref dst);
                    var writer = new BinaryWriter(dst, length);
                    if (v != null)
                    {
                        v.WriteToFaster(writer);
                    }
                    SetHeader(ref dst, writer);
                    SetPos(ref dst, writer);
                    return true;
                }
                return false;
            }

            public unsafe TValue ReadOutput(ref byte* src)
            {
                var length = Unsafe.Read<int>(src);
                src += sizeof(int);
                var reader = new BinaryReader(ref src, length);
                return TValue.ReadFromFaster(reader);
            }

            public unsafe TKKey ReadKey(ref byte* src)
            {
                return TKey.DeserializeKey(new BinaryReader(ref src, ushort.MaxValue));
            }

            public unsafe TValue ReadValue(ref byte* src)
            {
                var length = Unsafe.Read<int>(src);
                src += sizeof(int);
                var reader = new BinaryReader(ref src, length);
                return TValue.ReadFromFaster(reader);
            }

            private unsafe void ReserveHeader(ref byte* dst)
            {
                dst += sizeof(int);
            }

            // TODO, are these Masks really needed?
            const int KHeaderMask = 0x3 << 30;
            const int KUnserializedBitMask = 1 << 31;

            private unsafe void SetHeader(ref byte* dst, IBufferWriter writer)
            {
                dst -= sizeof(int); // we go back 4 bytes
                var length = (int)writer.Position;
                if (length == 1024)
                {
                    string a = "";
                }
                *(int*) dst = length;// (length | KUnserializedBitMask) & ~KHeaderMask;
                dst += sizeof(int); // we go forward 4 bytes
            }

            private unsafe void SetPos(ref byte* dst, IBufferWriter writer)
            {
                dst += writer.Position;
            }

            private unsafe void ReadHeader(ref byte* dst, IBufferWriter writer)
            {
                var length = (int)writer.Position;
                *(int*)dst = (length & KHeaderMask);
            }
        }
    }
}
