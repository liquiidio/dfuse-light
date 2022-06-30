using System.Diagnostics;
using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure;
using FASTER.common;

namespace DeepReader.Storage.Faster.Transactions.Server
{
    internal class TransactionKeyInputSerializer : IKeyInputSerializer<TransactionId, TransactionId>
    {
        [ThreadStatic]
        private static TransactionId _key;

        [ThreadStatic]
        private static TransactionId _input;

        public bool Match(ref TransactionId k, bool asciiKey, ref TransactionId pattern, bool asciiPattern)
        {
            // I have no idea what needs to be done here ...

            if(k.Id.Binary.SequenceEqual(pattern.Id.Binary))
                return true;
            else
            {
                // TODO
                Console.WriteLine("Match false");
            }
            return true;
        }

        // ref-return-value does not need to be blittable
        public unsafe ref TransactionId ReadInputByRef(ref byte* src)
        {
            var reader = new UnsafeBinaryUnmanagedReader(ref src, Checksum256.Checksum256ByteLength);
            var trxId = Types.Eosio.Chain.TransactionId.ReadFromBinaryReader(reader);
            _input = new TransactionId(trxId);
            return ref _input;
        }

        // ref-return-value does not need to be blittable
        public unsafe ref TransactionId ReadKeyByRef(ref byte* src)
        {
            var reader = new UnsafeBinaryUnmanagedReader(ref src, Checksum256.Checksum256ByteLength);
            var trxId = Types.Eosio.Chain.TransactionId.ReadFromBinaryReader(reader);
            _key = new TransactionId(trxId);
            return ref _key;
        }
    }
}