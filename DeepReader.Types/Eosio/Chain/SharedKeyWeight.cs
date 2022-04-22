using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public class SharedKeyWeight : IEosioSerializable<SharedKeyWeight>
{
    public PublicKey Key;   // for now public key, is SharedPublicKey in EOSIO (see below)
    public WeightType Weight;

    public SharedKeyWeight(BinaryReader reader)
    {
        Key = reader.ReadPublicKey();
        Weight = reader.ReadUInt16();
    }
    public static SharedKeyWeight ReadFromBinaryReader(BinaryReader reader)
    {
        return new SharedKeyWeight(reader);
    }
}