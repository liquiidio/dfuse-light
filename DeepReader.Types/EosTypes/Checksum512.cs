using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;
using DeepReader.Types.Other;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(Checksum512JsonConverter))]
public sealed class Checksum512 : PooledObject<Checksum512>, IEosioSerializable<Checksum512>
{
    private const int Checksum512ByteLength = 64;

    [JsonIgnore]
    public byte[] Binary { get; set; } = Array.Empty<byte>();

    private string? _stringVal;

    public string StringVal
    {
        get => _stringVal ??= SerializationHelper.ByteArrayToHexString(Binary);
        set => _stringVal = value;
    }

    public static Checksum512 ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new Checksum512();

        obj.Binary = reader.ReadBytes(Checksum512ByteLength);

        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Binary);
        _stringVal = null;

        ReturnToPool(this);
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