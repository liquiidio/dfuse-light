using System.Text.Json.Serialization;
using DeepReader.Types.Fc;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(Checksum256JsonConverter))]
public class Checksum256 : BinaryType
{
    private string _value = string.Empty;

    public static implicit operator Checksum256(string value)
    {
        return new Checksum256{ _value = value };
    }

    public static implicit operator string(Checksum256 value)
    {
        return value._value;
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