using System.Text.Json.Serialization;
using DeepReader.Types.Fc;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(Checksum160JsonConverter))]
public sealed class Checksum160 : BinaryType
{
    private string? _stringVal;

    public string StringVal
    {
        get => _stringVal ??= SerializationHelper.ByteArrayToHexString(Binary);
        set => _stringVal = value;
    }

    public static implicit operator string(Checksum160 value)
    {
        return value.StringVal;
    }

    public static implicit operator Checksum160(byte[] binary)
    {
        return new Checksum160 { Binary = binary };
    }

    public static implicit operator byte[](Checksum160 value)
    {
        return value.Binary;
    }

    public static Checksum160 Empty => new();
}