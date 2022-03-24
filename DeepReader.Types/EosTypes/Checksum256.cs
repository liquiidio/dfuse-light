using System.Text.Json.Serialization;
using DeepReader.Types.Fc;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(Checksum256JsonConverter))]
public class Checksum256 : BinaryType
{
    private string? _stringVal = string.Empty;

    public string StringVal
    {
        get => _stringVal ??= SerializationHelper.ByteArrayToHexString(Binary);
        set => _stringVal = value;
    }

    public static implicit operator Checksum256(string value)
    {
        return new Checksum256{ _stringVal = value };
    }

    public static implicit operator string(Checksum256 value)
    {
        return value.StringVal;
    }

    public static implicit operator Checksum256(byte[] binary)
    {
        return new Checksum256 { Binary = binary };
    }

    public static implicit operator byte[](Checksum256 value)
    {
        return value.Binary;
    }

    public static Checksum256 Empty => new();
}