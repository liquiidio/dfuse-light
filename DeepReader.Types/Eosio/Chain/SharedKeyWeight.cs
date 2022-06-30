using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public sealed class SharedKeyWeight : IEosioSerializable<SharedKeyWeight>
{
    public PublicKey Key;   // for now public key, is SharedPublicKey in EOSIO (see below)
    public WeightType Weight;

    public SharedKeyWeight(IBufferReader reader)
    {
        Key = PublicKey.ReadFromBinaryReader(reader);
        Weight = reader.ReadUInt16();
    }
    public static SharedKeyWeight ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new SharedKeyWeight(reader);
    }
}