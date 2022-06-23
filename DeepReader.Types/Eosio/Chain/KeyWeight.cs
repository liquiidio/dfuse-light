using DeepReader.Types.EosTypes;
using Salar.BinaryBuffers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public sealed class KeyWeight : IEosioSerializable<KeyWeight>
{
    public PublicKey Key = PublicKey.TypeEmpty;//string
    public WeightType Weight = 0;//uint32

    public KeyWeight() { }

    public KeyWeight(BinaryBufferReader reader)
    {
        Key = PublicKey.ReadFromBinaryReader(reader);
        Weight = reader.ReadUInt16();
    }

    public static KeyWeight ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return new KeyWeight(reader);
    }
}