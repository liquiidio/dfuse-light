using DeepReader.Types.Infrastructure.BinaryReaders;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public sealed class PermissionLevelWeight : IEosioSerializable<PermissionLevelWeight>
{
    public PermissionLevel Permission = PermissionLevel.TypeEmpty;//*PermissionLevel
    public WeightType Weight = 0;//uint32

    public PermissionLevelWeight() { }

    public PermissionLevelWeight(IBufferReader reader)
    {
        Permission = PermissionLevel.ReadFromBinaryReader(reader);
        Weight = reader.ReadUInt16();
    }

    public static PermissionLevelWeight ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new PermissionLevelWeight(reader);
    }
}