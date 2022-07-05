using DeepReader.Types.Extensions;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Interfaces;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// /// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public abstract class BlockSigningAuthorityVariant : IEosioSerializable<BlockSigningAuthorityVariant>
{
    public static BlockSigningAuthorityVariant ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        var type = reader.ReadByte();
        switch (type)
        {
            case 0:
                return BlockSigningAuthorityV0.ReadFromBinaryReader(reader);
            default:
                throw new Exception($"BlockSigningAuthorityVariant {type} unknown");
        }
    }
}

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public sealed class BlockSigningAuthorityV0 : BlockSigningAuthorityVariant, IEosioSerializable<BlockSigningAuthorityV0>
{
    public uint Threshold;
    public SharedKeyWeight[] Keys;

    public BlockSigningAuthorityV0(IBufferReader reader)
    {
        Threshold = reader.ReadUInt32();

        Keys = new SharedKeyWeight[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Keys.Length; i++)
        {
            Keys[i] = SharedKeyWeight.ReadFromBinaryReader(reader);
        }
    }

    public new static BlockSigningAuthorityV0 ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new BlockSigningAuthorityV0(reader);
    }
}