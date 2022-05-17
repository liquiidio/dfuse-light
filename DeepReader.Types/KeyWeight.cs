using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public class KeyWeight : IEosioSerializable<KeyWeight>
{
    public PublicKey Key = PublicKey.TypeEmpty;//string
    public uint Weight = 0;//uint32

    public KeyWeight() { }

    public KeyWeight(BinaryReader reader)
    {
        Key = PublicKey.ReadFromBinaryReader(reader);
        Weight = reader.ReadUInt32();
    }

    public static KeyWeight ReadFromBinaryReader(BinaryReader reader)
    {
        return new KeyWeight(reader);
    }
}