using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// Variant<TransactionId, PackedTransaction>
/// </summary>
public abstract class TransactionVariant : IEosioSerializable<TransactionVariant>, IFasterSerializable<TransactionVariant>
{
    public static TransactionVariant ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        var type = reader.ReadByte();
        switch (type)
        {
            case 0:
                return TransactionId.ReadFromBinaryReader(reader, fromPool);
            case 1:
                return PackedTransaction.ReadFromBinaryReader(reader, fromPool);
            default:
                throw new Exception("BlockSigningAuthorityVariant {type} unknown");
        }
    }

    public static TransactionVariant ReadFromFaster(IBufferReader reader, bool fromPool = true)
    {
        var type = reader.ReadByte();
        switch (type)
        {
            case 0:
                return TransactionId.ReadFromFaster(reader, fromPool);
            case 1:
                return PackedTransaction.ReadFromFaster(reader, fromPool);
            default:
                throw new Exception("BlockSigningAuthorityVariant {type} unknown");
        }
    }

    public void WriteToFaster(IBufferWriter writer)
    {
        if (this is TransactionId transactionId)
        {
            writer.Write((byte)0);
            writer.Write(transactionId.Binary);
        }
        else if (this is PackedTransaction packedTransaction)
        {
            writer.Write((byte)1);
            packedTransaction.WriteToFaster(writer);
        }
    }
}