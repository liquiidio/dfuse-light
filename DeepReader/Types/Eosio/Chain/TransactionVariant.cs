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
    private readonly string _value = string.Empty;

    public TransactionId()
    {
        _value = string.Empty;
    }

    public TransactionId(string value)
    {
        _value = value;
    }

    public static implicit operator TransactionId(string value)
    {
        return new TransactionId(value);
    }

    public static implicit operator string(TransactionId value)
    {
        return value._value;
    }

    public string ToJson()
    {
        return _value;
    }

    public override string ToString()
    {
        return _value;
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