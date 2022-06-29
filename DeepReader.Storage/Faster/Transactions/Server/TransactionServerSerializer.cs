using System.Runtime.CompilerServices;
using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Types.EosTypes;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;
using KGySoft.CoreLibraries;

namespace DeepReader.Storage.Faster.Transactions.Server
{
    internal class TransactionServerSerializer : IServerSerializer<TransactionId, TransactionTrace, TransactionId, TransactionTrace>
    {
        [ThreadStatic]
        private static TransactionId _input;

        [ThreadStatic]
        private static TransactionTrace _output;

        [ThreadStatic]
        private static TransactionId _key;

        [ThreadStatic] 
        private static TransactionTrace _value;

        // all methods here are used in WebsocketServerSession

        // TransactionTrace is Output
        // seems like the ref-output is never really used and after some internal calls just passed to xxxServerFunctions while calling SingleReader
        // seems to be a ServerSide-only/Server-internal READ-operation
        public unsafe ref TransactionTrace AsRefOutput(byte* dcurr, int length)
        {
            // ### copied from ..src/FASTER.server/SpanByteServerSerializer.cs
            //_output = SpanByteAndMemory.FromFixedSpan(new Span<byte>(src, length));
            //return ref _output;

            //var transactionTraceLength = Unsafe.Read<long>(src);
            //src += sizeof(long);

            // TODO, not sure if this has length-prefix or not
            // this is read from the store so it doesn't have the length-prefix 
            var reader = new BinaryReader(new UnmanagedMemoryStream(dcurr, length));
            _output = TransactionTrace.ReadFromBinaryReader(reader);
            dcurr += reader.BaseStream.Position; // seems like this is not needed as sry is no ref
            return ref _output;

            //output = SpanByteAndMemory.FromFixedSpan(new Span<byte>(src, (int)reader.BaseStream.Position));
            //return ref output;

            //            src += reader.BaseStream.Position;
            //var output = reader.BaseStream.ToArray();
            //var p = (IntPtr)((byte*)output[0]);
            //return ref new SpanByte(0, (IntPtr)((byte*)output[0]));
        }

        //ref Output
        // no reference fount that calls this method
        // TransactionTrace is Output
        public int GetLength(ref TransactionTrace o)
        {
            var writer = new BinaryWriter(new MemoryStream());
            o.WriteToBinaryWriter(writer);
            return (int)writer.BaseStream.Position;
        }

        // seems like the ref-output is never really used and after som
        // e internal calls just passed to xxxServerFunctions while calling SingleReader
        // looks like Input could be of type Key
        // TransactionId is Input
        public unsafe ref TransactionId ReadInputByRef(ref byte* src)
        {
            // ### copied from ..src/FASTER.server/SpanByteServerSerializer.cs
            //ref var ret = ref Unsafe.AsRef<SpanByte>(src);
            //src += ret.TotalSize;
            //return ref ret;

            // length is static, no need to read or write it for TransactionId
            var reader = new BinaryReader(new UnmanagedMemoryStream(src, Checksum256.Checksum256ByteLength));
            var trxId = Types.Eosio.Chain.TransactionId.ReadFromBinaryReader(reader);
            _input = new TransactionId(trxId);
            src += Checksum256.Checksum256ByteLength;
            return ref _input;
        }

        // seems like the ref-output is never really used and after some internal calls just passed to xxxServerFunctions while calling diverse Methods
        public unsafe ref TransactionId ReadKeyByRef(ref byte* src) // src is the bytes received over websocket
        {
            var reader = new BinaryReader(new UnmanagedMemoryStream(src, Checksum256.Checksum256ByteLength + sizeof(int)));
            var length = reader.ReadInt32();
            // Todo, we could verify the size here
            var trxId = Types.Eosio.Chain.TransactionId.ReadFromBinaryReader(reader);
            _key = new TransactionId(trxId);
            src += Checksum256.Checksum256ByteLength + sizeof(int);
            return ref _key;
        }

        // seems like the ref-output is never really used and after some internal calls just passed to xxxServerFunctions while calling diverse Methods
        public unsafe ref TransactionTrace ReadValueByRef(ref byte* src) // src is the bytes received over websocket
        {
            //var length = Unsafe.Read<long>(src); // read length of TransactionTrace in bytes
            //src += sizeof(long); // increase pointer by size of long

            // this is read from the store so it doesn't have the length-prefix 
            var reader = new BinaryReader(new UnmanagedMemoryStream(src, ushort.MaxValue));
            var length = reader.ReadInt32();
            _value = TransactionTrace.ReadFromBinaryReader(reader);
            src += reader.BaseStream.Position;
            return ref _value;
        }

        // not sure, seems to just set the src-pointer to something?!
        public unsafe void SkipOutput(ref byte* dcurr)
        {
            // TODO, not sure if this has length-prefix or not
            var length = Unsafe.Read<long>(dcurr); // read length of TransactionTrace in bytes
            dcurr += sizeof(long); // increase pointer by size of long
            var reader = new BinaryReader(new UnmanagedMemoryStream(dcurr, length));
            TransactionTrace.ReadFromBinaryReader(reader);
            dcurr += length;
        }

        // Write Key to Destination dst*
        public unsafe bool Write(ref TransactionId k, ref byte* dst, int length) // length is max memory available/left
        {
            // We don't write the length here as Key is fixed length
            var writer = new BinaryWriter(new UnmanagedMemoryStream(dst, length));
            k.Id.WriteToBinaryWriter(writer); // serialize !
            dst += writer.BaseStream.Position;

            return true;
        }

        // Write Value to Destination dst*
        // TransactionTrace is output and Value
        public unsafe bool Write(ref TransactionTrace v, ref byte* dst, int length) // length is max memory available/left
        {
            var stream = new UnmanagedMemoryStream(dst, length);
            var writer = new BinaryWriter(stream);
            writer.BaseStream.Position += sizeof(long); // reserve sizeof(long) so we can write the length after serialization
            v.WriteToBinaryWriter(writer); // serialize !
            Unsafe.Write(dst, writer.BaseStream.Position - sizeof(ulong));  // write length into reserved memory
            dst += writer.BaseStream.Position;  // set pointer to new end

            return true;
        }

        // Write Output to Destination dst*
        // Looks like Output could be of type Value
        //public unsafe bool Write(ref TransactionTrace k, ref byte* dst, int length)
        //{
        //    if (k.Length > length) return false;

        //    var dest = new SpanByte(length, (IntPtr)dst);
        //    if (k.IsSpanByte)
        //        k.SpanByte.CopyTo(ref dest);
        //    else
        //        k.Memory.Memory.Span.CopyTo(dest.AsSpan());
        //    return true;
        //}
    }
}