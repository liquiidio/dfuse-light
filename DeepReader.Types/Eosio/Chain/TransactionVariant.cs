using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// Variant<TransactionId, PackedTransaction>
/// </summary>
public abstract class TransactionVariant : IEosioSerializable<TransactionVariant>
{
    public static TransactionVariant ReadFromBinaryReader(BinaryReader reader)
    {
        var type = reader.ReadByte();
        switch (type)
        {
            case 0:
                return reader.ReadTransactionId();
            case 1:
                return PackedTransaction.ReadFromBinaryReader(reader);
            default:
                throw new Exception("BlockSigningAuthorityVariant {type} unknown");
        }
    }
}


/// <summary>
/// Custom type due to Variant-Handling
/// </summary>
[JsonConverter(typeof(TransactionIdJsonConverter))]
public class TransactionId : TransactionVariant
{
    public byte[] Binary = Array.Empty<byte>();

    private string? _stringVal;
    public string StringVal
    {
        get => _stringVal ??= SerializationHelper.ByteArrayToHexString(Binary);
        set => _stringVal = value;
    }

    public TransactionId()
    {

    }

    public TransactionId(string transactionId)
    {
        Binary = SerializationHelper.StringToByteArray(transactionId);
        _stringVal = transactionId;
    }

    public static implicit operator TransactionId(string value)
    {
        return new TransactionId(){ StringVal = value };
    }

    public static implicit operator string(TransactionId value)
    {
        return value.StringVal;
    }

    public static implicit operator TransactionId(byte[] binary)
    {
        return new TransactionId(){ Binary = binary };
    }

    public static implicit operator byte[](TransactionId value)
    {
        return value.Binary;
    }

    public override string ToString()
    {
        return StringVal;
    }

    public static TransactionId Empty => new();
}