using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;
using DeepReader.Types.Other;

namespace DeepReader.Types.Fc.Crypto;

[JsonConverter(typeof(SignatureJsonConverter))]
public sealed class Signature : PooledObject<Signature>, IEosioSerializable<Signature>
{
    const int SignKeyDataSize = 64;

    [JsonIgnore]
    private byte Type;
    public byte SomeByte { get; set; }
    public byte[] SignBytes { get; set; }

    private string? _stringVal;

    public string StringVal
    {
        get => _stringVal ??= SerializationHelper.ByteArrayToHexString(SignBytes);
        set => _stringVal = value;
    }

    public Signature()
    {

    }

    public static Signature ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        var obj = fromPool ? TypeObjectPool.Get() : new Signature();

        obj.Type = reader.ReadByte();
        obj.SignBytes = reader.ReadBytes(Constants.SignKeyDataSize);
        obj.SomeByte = reader.ReadByte();
        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Type);
        writer.Write(SignBytes);
        writer.Write(SomeByte);

        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        TypeObjectPool.Return(this);
    }

    public static implicit operator Signature(string value)
    {
        return new Signature { _stringVal = value };
    }

    public static implicit operator string(Signature value)
    {
        return value.StringVal;
    }

    public static readonly Signature TypeEmpty = new();
}