using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;
using Serilog;

namespace DeepReader.Types.Extensions
{
    internal static class BinaryWriterExtensions
    {
        public static void WriteName(this BinaryWriter writer, Name name)
        {
            if (name.Binary.Length != 8)
                Log.Error($"Name {name.StringVal} Bytes.Lenght != 8 !");
            writer.Write(name.Binary);
        }
        
        public static void WriteTransactionId(this BinaryWriter writer, TransactionId id)
        {
            writer.Write(id.Binary);
        }
        
        public static void WriteChecksum256(this BinaryWriter writer, Checksum256 checksum256)
        {
            writer.Write(checksum256.Binary);
        }

        public static void WriteSignature(this BinaryWriter writer, Signature signature)
        {
            writer.Write(signature.Binary);
        }
    }
}
