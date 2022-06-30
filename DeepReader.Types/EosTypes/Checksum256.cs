using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;
using DeepReader.Types.Other;
using Salar.BinaryBuffers;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(Checksum256JsonConverter))]
public sealed class Checksum256 : PooledObject<Checksum256>, IEosioSerializable<Checksum256>, IFasterSerializable<Checksum256>
{
    public const int Checksum256ByteLength = 32; // TODO different names for constants in general

    [JsonIgnore]
    public byte[] Binary { get; set; } = Array.Empty<byte>();

    private string? _stringVal;

    public string StringVal
    {
        get => _stringVal ??= SerializationHelper.ByteArrayToHexString(Binary);
        set => _stringVal = value;
    }

    public static Checksum256 ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new Checksum256();

        obj.Binary = reader.ReadBytes(Checksum256ByteLength);

        return obj;
    }

    public static Checksum256 ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new Checksum256();

        obj.Binary = reader.ReadBytes(Checksum256ByteLength);

        return obj;
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        writer.Write(Binary);
        _stringVal = null;
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

    public static readonly Checksum256 TypeEmpty = new();
}