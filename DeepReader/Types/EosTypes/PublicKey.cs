using System.Text.Json.Serialization;
using DeepReader.JsonConverters;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(PublicKeyJsonConverter))]
public class PublicKey
{
    private readonly string _value;

    public static implicit operator PublicKey(string value)
    {
        return new PublicKey(value);
    }

    public static implicit operator string(PublicKey value)
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

    public PublicKey(string value)
    {
        _value = value;
    }

    public static PublicKey Empty => new("");
}