using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types.Fc.Crypto;

[JsonConverter(typeof(SignatureJsonConverter))]
public class Signature : BinaryType
{
    private string? _stringVal = string.Empty;

    public string StringVal
    {
        get => _stringVal ??= SerializationHelper.ByteArrayToHexString(Binary);
        set => _stringVal = value;
    }

    public static implicit operator Signature(string value)
    {
        return new Signature { _stringVal = value };
    }

    public static implicit operator string(Signature value)
    {
        return value.StringVal;
    }

    public static implicit operator Signature(byte[] binary)
    {
        return new Signature { Binary = binary };
    }

    public static implicit operator byte[](Signature value)
    {
        return value.Binary;
    }

    public static Signature Empty => new();
}