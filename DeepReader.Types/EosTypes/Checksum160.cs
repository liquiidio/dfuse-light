using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;
using DeepReader.Types.Other;
using Salar.BinaryBuffers;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(Checksum160JsonConverter))]
public sealed class Checksum160 : PooledObject<Checksum160>, IEosioSerializable<Checksum160>
{
    private const int Checksum160ByteLength = 20;

    [JsonIgnore]
    public byte[] Binary { get; set; } = Array.Empty<byte>();

    private string? _stringVal;

    public string StringVal
    {
        get => _stringVal ??= SerializationHelper.ByteArrayToHexString(Binary);
        set => _stringVal = value;
    }

    public static Checksum160 ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new Checksum160();

        obj.Binary = reader.ReadBytes(Checksum160ByteLength);

        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Binary);
        _stringVal = null;
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