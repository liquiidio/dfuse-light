using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure.BinaryReaders;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public sealed class KeyWeight : IEosioSerializable<KeyWeight>
{
    public PublicKey Key = PublicKey.TypeEmpty;//string
    public WeightType Weight = 0;//uint32

    public KeyWeight() { }

    public KeyWeight(IBufferReader reader)
    {
        Key = PublicKey.ReadFromBinaryReader(reader);
        Weight = reader.ReadUInt16();
    }

    public static KeyWeight ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new KeyWeight(reader);
    }
}