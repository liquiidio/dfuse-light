using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action.hpp
/// </summary>
public class PermissionLevel : IEosioSerializable<PermissionLevel>
{
    public Name Actor;
    public Name Permission;

    public PermissionLevel(BinaryReader reader)
    {
        Actor = reader.ReadName();
        Permission = reader.ReadName();
    }

    public static PermissionLevel ReadFromBinaryReader(BinaryReader reader)
    {
        return new PermissionLevel(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.WriteName(Actor);
        writer.WriteName(Permission);
    }
}