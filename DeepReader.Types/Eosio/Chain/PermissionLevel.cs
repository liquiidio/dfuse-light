using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action.hpp
/// </summary>
public class PermissionLevel
{
    public Name Actor = Name.Empty;
    public Name Permission = Name.Empty;

    public static PermissionLevel ReadFromBinaryReader(BinaryReader reader)
    {
        PermissionLevel level = new PermissionLevel()
        {
            Actor = reader.ReadName(),
            Permission = reader.ReadName()
        };
        return level;
    }
}