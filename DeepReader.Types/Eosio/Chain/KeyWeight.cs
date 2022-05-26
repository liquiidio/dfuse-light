using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public sealed class KeyWeight : IEosioSerializable<KeyWeight>
{
    public PublicKey Key = PublicKey.TypeEmpty;//string
    public WeightType Weight = 0;//uint32

    public KeyWeight() { }

    public KeyWeight(BinaryReader reader)
    {
        Key = PublicKey.ReadFromBinaryReader(reader);
        Weight = reader.ReadUInt16();
    }

    public static KeyWeight ReadFromBinaryReader(BinaryReader reader)
    {
        return new KeyWeight(reader);
    }
}