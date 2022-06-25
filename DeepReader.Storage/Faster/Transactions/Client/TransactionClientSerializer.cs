﻿using System.Runtime.CompilerServices;
using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.common;

namespace DeepReader.Storage.Faster.Transactions.Client
{
    // notes Output is TransactionTrace or Wrapper, must be same type as Output in TransactionClientFunctions
    // FASTER.client.ClientSession<T> internally calls ReadOutput and then ClientFunctions.ReadCompletionCallback etc.
    internal class TransactionClientSerializer : IClientSerializer<TransactionId, TransactionTrace, TransactionInput, TransactionTrace>
    {
        // Write(ref Key)
        // almost always used with Write(ref Value) or Write(ref Input)
        // used standalone in InternalSubscribe, InternalDelete
        // length seems to be the overal size left for the message
        public unsafe bool Write(ref TransactionId k, ref byte* dst, int length)
        {
            var stream = new UnmanagedMemoryStream(dst, length);
            var writer = new BinaryWriter(stream);
            k.Id.WriteToBinaryWriter(writer);
            // TODO the writer/stream internally increasing the pointer?
            // TODO do we need to add the length here? As Checksum256 is fixed size?

            return true;
        }

        // Write(ref Value)
        // InternalUpsert, InternalPublish
        public unsafe bool Write(ref TransactionTrace v, ref byte* dst, int length)
        {
            // TODO hmhm, we need to add the length here first ?
            var stream = new UnmanagedMemoryStream(dst, length);
            var writer = new BinaryWriter(stream);
            v.WriteToBinaryWriter(writer);
            // TODO the writer/stream internally increasing the pointer?

            return true;
        }

        // Write(ref Input)
        // InternalRead, InternalSubscribeKV, InternalRMW
        // -> messageType
        //  -> serialNo
        //   -> key
        //    -> input
        public unsafe bool Write(ref TransactionInput i, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }

        public unsafe TransactionTrace ReadOutput(ref byte* src)
        {
            var length = Unsafe.Read<int>(src);
            src += sizeof(int);
            // TODO we need to add the length on Server
            // TODO 2 is Unsafe.Read<T> increasing the pointer?
            var reader = new BinaryReader(new UnmanagedMemoryStream(src, length));
            src += length;
            return TransactionTrace.ReadFromBinaryReader(reader);
        }

        // ReadKey and ReadValue seem to only be used in Subscriptions (e.g in Subscription-events in ClientSession

        public unsafe TransactionId ReadKey(ref byte* src)
        {
            throw new NotImplementedException();
        }

        // ReadKey and ReadValue seem to only be used in Subscriptions (e.g in Subscription-events in ClientSession
        public unsafe TransactionTrace ReadValue(ref byte* src)
        {
            throw new NotImplementedException();
        }
    }
}