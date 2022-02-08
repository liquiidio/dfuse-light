using System.Text.Json.Serialization;
using DeepReader.JsonConverters;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(Checksum256JsonConverter))]
public class Checksum256
{
    private readonly string _value = string.Empty;

    public static implicit operator Checksum256(string value)
    {
        return new Checksum256(value);
    }

    public static implicit operator string(Checksum256 value)
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

    public Checksum256(string value)
    {
        _value = value;
    }

    public static Checksum256 Empty => new("");
}