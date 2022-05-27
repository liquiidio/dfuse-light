namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// Variant<TransactionId, PackedTransaction>
/// </summary>
public abstract class TransactionVariant : IEosioSerializable<TransactionVariant>
{
    public static TransactionVariant ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
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

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        if (this is TransactionId transactionId)
        {
            writer.Write((byte)0);
            writer.Write(transactionId.Binary);
        }
        else if (this is PackedTransaction packedTransaction)
        {
            writer.Write((byte)1);
            packedTransaction.WriteToBinaryWriter(writer);
        }
    }
}