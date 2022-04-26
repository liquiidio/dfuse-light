using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action.hpp
/// </summary>
public class PermissionLevel : IEosioSerializable<PermissionLevel>
{
    public Name Actor { get; set; }
    public Name Permission { get; set; }

    public PermissionLevel()
    {
        Actor = 0;
        Permission = 0;
    }

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