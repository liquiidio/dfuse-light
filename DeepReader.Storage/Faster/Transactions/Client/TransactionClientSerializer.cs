using System.Runtime.CompilerServices;
using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure;
using DeepReader.Types.StorageTypes;
using FASTER.common;

namespace DeepReader.Storage.Faster.Transactions.Client
{
    // notes Output is TransactionTrace or Wrapper, must be same type as Output in TransactionClientFunctions
    // FASTER.client.ClientSession<T> internally calls ReadOutput and then ClientFunctions.ReadCompletionCallback etc.
    internal class TransactionClientSerializer : IClientSerializer<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace>
    {
        // Write(ref Key)
        // almost always used with Write(ref Value) or Write(ref Input)
        // used standalone in InternalSubscribe, InternalDelete
        // length seems to be the overal size left for the message
        public unsafe bool Write(ref TransactionId k, ref byte* dst, int length)
        {
            ReserveHeader(ref dst); // we need to reserve an sizeof(int) here
            var writer = new BinaryWriter(new UnmanagedMemoryStream(dst, length - sizeof(int), length - sizeof(int), FileAccess.Write));
            k.Id.WriteToFaster(writer);
            SetHeader(ref dst, writer);
            SetPos(ref dst, writer);
            return true;
        }

        // typeof(Value) == typeof(Input) in our case so only one Write-method for both
        // Write(ref Value)
        // InternalUpsert, InternalPublish
        // Write(ref Input)
        // InternalRead, InternalSubscribeKV, InternalRMW
        // -> messageType
        //  -> serialNo
        //   -> key
        //    -> input
        public unsafe bool Write(ref TransactionTrace v, ref byte* dst, int length)
        {
            ReserveHeader(ref dst); // we need to reserve an sizeof(int) here
            var writer = new BinaryWriter(new UnmanagedMemoryStream(dst, length - sizeof(int), length - sizeof(int), FileAccess.Write));
            v.WriteToFaster(writer);
            SetHeader(ref dst, writer);
            SetPos(ref dst, writer);
            return true;
        }

        public unsafe TransactionTrace ReadOutput(ref byte* src)
        {
            var length = Unsafe.Read<int>(src);
            src += sizeof(int);
            // length is written in TransactionServerSerializer.Write(...)
            var reader = new UnsafeBinaryUnmanagedReader(ref src, length);
            return TransactionTrace.ReadFromBinaryReader(reader);
        }

        // ReadKey and ReadValue seem to only be used in Subscriptions (e.g in Subscription-events in ClientSession

        public unsafe TransactionId ReadKey(ref byte* src)
        {
            // TODO clean this up
            var reader = new UnsafeBinaryUnmanagedReader(src, Checksum256.Checksum256ByteLength);
            var trxId = Types.Eosio.Chain.TransactionId.ReadFromBinaryReader(reader);
            var id = new TransactionId(trxId);
            return id;
        }

        // ReadKey and ReadValue seem to only be used in Subscriptions (e.g in Subscription-events in ClientSession
        public unsafe TransactionTrace ReadValue(ref byte* src)
        {
            // TODO clean this up
            var length = Unsafe.Read<int>(src);
            src += sizeof(int);
            // length is written in TransactionServerSerializer.Write(...)
            var reader = new UnsafeBinaryUnmanagedReader(src, length);
            var trace = TransactionTrace.ReadFromBinaryReader(reader);
            return trace;
        }


        private unsafe void ReserveHeader(ref byte* dst)
        {
            dst += sizeof(int);
        }

        const int kHeaderMask = 0x3 << 30;
        const int kUnserializedBitMask = 1 << 31;

        private unsafe void SetHeader(ref byte* dst, BinaryWriter writer)
        {
            dst -= sizeof(int); // we go back 4 bytes
            var length = (int)writer.BaseStream.Position;
            *(int*)dst = (length | kUnserializedBitMask) & ~kHeaderMask;
            dst += sizeof(int); // we go forward 4 bytes
        }

        private unsafe void SetPos(ref byte* dst, BinaryWriter writer)
        {
            dst += writer.BaseStream.Position;
        }

        private unsafe void ReadHeader(ref byte* dst, BinaryWriter writer)
        {
            var length = (int)writer.BaseStream.Position;
            *(int*)dst = (length & kHeaderMask);
        }
    }
}