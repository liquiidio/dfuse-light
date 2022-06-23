using System.Text.Json.Serialization;
using DeepReader.Types.Extensions;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;
using DeepReader.Types.Other;
using Salar.BinaryBuffers;
using Serilog;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(PublicKeyJsonConverter))]
public sealed class PublicKey : PooledObject<PublicKey>, IEosioSerializable<PublicKey>, IFasterSerializable<PublicKey>
{
    [JsonIgnore]
    public byte[] Binary { get; set; } = Array.Empty<byte>();

    private byte Type { get; set; }

    private string? _stringVal;

    public string StringVal {
        get => _stringVal ??= ToString();
        set => _stringVal = value;
    }

    public override string ToString()
    {
        switch (Type)
        {
            case (int)BinaryReaderExtensions.KeyType.K1:
                return CryptoHelper.PubKeyBytesToString(Binary, "K1");
            case (int)BinaryReaderExtensions.KeyType.R1:
                return CryptoHelper.PubKeyBytesToString(Binary, "R1", "PUB_R1_");
            case (int)BinaryReaderExtensions.KeyType.WA:
                return CryptoHelper.PubKeyBytesToString(Binary, "WA", "PUB_WA_");
            default:
                Log.Error(new Exception($"public key type {Type} not supported"), "");
                Log.Error(CryptoHelper.PubKeyBytesToString(Binary, "R1", "PUB_R1_"));
                return CryptoHelper.PubKeyBytesToString(Binary, "R1", "PUB_R1_"); // TODO ??
        }
    }

    public static implicit operator PublicKey(string value)
    {
        return new PublicKey { _stringVal = value };
    }

    public static implicit operator string(PublicKey value)
    {
        return value.StringVal;
    }

    public static implicit operator PublicKey(byte[] value)
    {
        return new PublicKey { Binary = value };
    }

    public static implicit operator byte[](PublicKey value)
    {
        return value.Binary;
    }

    public static readonly PublicKey TypeEmpty = new();

    public PublicKey()
    {
    }

    public static PublicKey ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new PublicKey();

        obj.Type = reader.ReadByte();
        obj.Binary = reader.ReadBytes(Constants.PubKeyDataSize);

        return obj;
    }

    public static PublicKey ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new PublicKey();

        obj.Type = reader.ReadByte();
        obj.Binary = reader.ReadBytes(Constants.PubKeyDataSize);

        return obj;
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        writer.Write(Type);
        writer.Write(Binary);
    }
}