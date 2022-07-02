using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.JsonConverters;
using DeepReader.Types.Other;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(Checksum512JsonConverter))]
public sealed class Checksum512 : PooledObject<Checksum512>, IEosioSerializable<Checksum512>, IFasterSerializable<Checksum512>
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

    public static Checksum512 ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new Checksum512();

        obj.Binary = reader.ReadBytes(Checksum512ByteLength);

        return obj;
    }

    public static Checksum512 ReadFromFaster(IBufferReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new Checksum512();

        obj.Binary = reader.ReadBytes(Checksum512ByteLength);

        return obj;
    }

    public void WriteToFaster(IBufferWriter writer)
    {
        writer.Write(Binary);
        _stringVal = null;
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