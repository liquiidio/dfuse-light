using System.Text.Json.Serialization;
using DeepReader.Types.Extensions;
using DeepReader.Types.Helpers;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;
using DeepReader.Types.JsonConverters;
using DeepReader.Types.Other;
using Serilog;

namespace DeepReader.Types.Fc.Crypto;

[JsonConverter(typeof(SignatureJsonConverter))]
public sealed class Signature : PooledObject<Signature>, IEosioSerializable<Signature>, IFasterSerializable<Signature>
{
    const int SignKeyDataSize = 65;

    [JsonIgnore]
    private byte Type;
    public byte SomeByte { get; set; }
    public byte[] SignBytes { get; set; }

    private string? _stringVal;

    public string StringVal
    {
        get => _stringVal ??= CryptoHelper.SignBytesToString(SignBytes);
        set => _stringVal = value;
    }

    public override string ToString()
    {
        switch (Type)
        {
            case (int)BinaryReaderExtensions.KeyType.R1:
                return CryptoHelper.SignBytesToString(SignBytes, "R1", "SIG_R1_");
            case (int)BinaryReaderExtensions.KeyType.K1:
                return CryptoHelper.SignBytesToString(SignBytes, "K1", "SIG_K1_");
            default:
                Log.Error(new Exception($"Signature type {Type} not supported"), "");
                Log.Error(new Exception(CryptoHelper.SignBytesToString(SignBytes, "K1", "SIG_K1_")), "");
                return CryptoHelper.SignBytesToString(SignBytes, "K1", "SIG_K1_");  // TODO ??
        }
    }

    public Signature()
    {

    }

    public static Signature ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        var obj = fromPool ? TypeObjectPool.Get() : new Signature();

        obj.Type = reader.ReadByte();
        obj.SignBytes = reader.ReadBytes(SignKeyDataSize);
        return obj;
    }

    public static Signature ReadFromFaster(IBufferReader reader, bool fromPool = true)
    {
        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        var obj = fromPool ? TypeObjectPool.Get() : new Signature();

        obj.Type = reader.ReadByte();
        obj.SignBytes = reader.ReadBytes(SignKeyDataSize);
        return obj;
    }

    public void WriteToFaster(IBufferWriter writer)
    {
        writer.Write(Type);
        writer.Write(SignBytes);
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