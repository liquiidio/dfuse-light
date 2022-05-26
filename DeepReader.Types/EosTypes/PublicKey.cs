using System.Text.Json.Serialization;
using DeepReader.Types.Fc;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(PublicKeyJsonConverter))]
public sealed class PublicKey : BinaryType, IEosioSerializable<PublicKey>
{
    private string _stringVal = string.Empty;

    public string StringVal {
        get => _stringVal ??= CryptoHelper.PubKeyBytesToString(Binary);
        set => _stringVal = value;
    }

    public static implicit operator PublicKey(string value)
    {
        return new PublicKey { _stringVal = value };
    }

    public static implicit operator string(PublicKey value)
    {
        return value._stringVal;
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

    public PublicKey(BinaryReader reader)
    {
        var type = reader.ReadByte();// TODO
        Binary = reader.ReadBytes(Constants.PubKeyDataSize);

        // TODO we don't need to deserialize/convert to string just for the deserialization of dlogs
        // but we probably need when returning data via API

        //switch (type)
        //{
        //    case (int)KeyType.K1:
        //        return CryptoHelper.PubKeyBytesToString(keyBytes, "K1");
        //    case (int)KeyType.R1:
        //        return CryptoHelper.PubKeyBytesToString(keyBytes, "R1", "PUB_R1_");
        //    case (int)KeyType.WA:
        //        return CryptoHelper.PubKeyBytesToString(keyBytes, "WA", "PUB_WA_");
        //    default:
        //        Log.Error(new Exception($"public key type {type} not supported"), "");
        //        Log.Error(CryptoHelper.PubKeyBytesToString(keyBytes, "R1", "PUB_R1_"));
        //        return CryptoHelper.PubKeyBytesToString(keyBytes, "R1", "PUB_R1_"); // TODO ??
        //}
    }

    public static PublicKey ReadFromBinaryReader(BinaryReader reader)
    {
        return new PublicKey(reader);
    }
}