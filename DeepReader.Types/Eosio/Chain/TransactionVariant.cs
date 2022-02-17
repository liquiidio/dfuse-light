using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// Variant<TransactionId, PackedTransaction>
/// </summary>
public abstract class TransactionVariant
{

}


/// <summary>
/// Custom type due to Variant-Handling
/// </summary>
public class TransactionId : TransactionVariant
{
    public byte[] Binary = Array.Empty<byte>();

    public string Value = string.Empty; // TODO StringVal etc. BinaryType?!

    public TransactionId()
    {
        Value = string.Empty;
    }

    public static implicit operator TransactionId(string value)
    {
        return new TransactionId(){ Value = value };
    }

    public static implicit operator string(TransactionId value)
    {
        return value.Value;
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
        return Value;
    }

    public static TransactionId Empty => new();
}

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public class PackedTransaction : TransactionVariant
{
    public Signature[] Signatures = Array.Empty<Signature>();
    public byte Compression = 0; //fc::enum_type<uint8_t, compression>
    public Bytes PackedContextFreeData = new();
    public Bytes PackedTrx = new ();
}