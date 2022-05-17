using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Types;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public class PermissionLevelWeight : IEosioSerializable<PermissionLevelWeight>
{
    public PermissionLevel Permission = PermissionLevel.TypeEmpty;//*PermissionLevel
    public uint Weight = 0;//uint32

    public PermissionLevelWeight() { }

    public PermissionLevelWeight(BinaryReader reader)
    {
        Permission = PermissionLevel.ReadFromBinaryReader(reader);
        Weight = reader.ReadUInt32();
    }

    public static PermissionLevelWeight ReadFromBinaryReader(BinaryReader reader)
    {
        return new PermissionLevelWeight(reader);
    }
}