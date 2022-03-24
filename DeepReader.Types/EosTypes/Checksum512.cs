using System.Text.Json.Serialization;
using DeepReader.Types.Fc;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(Checksum512JsonConverter))]
public class Checksum512 : BinaryType
{
    private string? _stringVal = string.Empty;

    public string StringVal
    {
        get => _stringVal ??= SerializationHelper.ByteArrayToHexString(Binary);
        set => _stringVal = value;
    }

    public static implicit operator Checksum512(string value)
    {
        return new Checksum512 { _stringVal = value };
    }

    public static implicit operator string(Checksum512 value)
    {
        return value.StringVal;
    }

    public static implicit operator Checksum512(byte[] binary)
    {
        return new Checksum512 { Binary = binary };
    }

    public static implicit operator byte[](Checksum512 value)
    {
        return value.Binary;
    }

    public static Checksum512 Empty => new();
}