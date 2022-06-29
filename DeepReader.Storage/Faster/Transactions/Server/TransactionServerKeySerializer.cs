using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Types.EosTypes;
using FASTER.common;

namespace DeepReader.Storage.Faster.Transactions.Server
{
    internal class TransactionServerKeySerializer : IKeySerializer<TransactionId>
    {
        [ThreadStatic]
        static TransactionId _key;

        public bool Match(ref TransactionId k, bool asciiKey, ref TransactionId pattern, bool asciiPattern)
        {
            // I have no idea what needs to be done here ...

            if (k.Id.Binary.SequenceEqual(pattern.Id.Binary))
                return true;
            else
            {
                // TODO
                Console.WriteLine("Match false");
            }
            return true;
        }

        public unsafe ref TransactionId ReadKeyByRef(ref byte* src)
        {
            var reader = new BinaryReader(new UnmanagedMemoryStream(src, Checksum256.Checksum256ByteLength));
            var trxId = Types.Eosio.Chain.TransactionId.ReadFromBinaryReader(reader);
            _key = new TransactionId(trxId);
            src += Checksum256.Checksum256ByteLength;
            return ref _key;
        }
    }
}