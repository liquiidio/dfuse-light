using DeepReader.Types.EosTypes;
using DeepReader.Types.Other;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action.hpp
/// </summary>
public sealed class PermissionLevel : PooledObject<PermissionLevel>, IEosioSerializable<PermissionLevel>
{
    public Name Actor { get; set; }
    public Name Permission { get; set; }

    public PermissionLevel()
    {
        Actor = Name.TypeEmpty;
        Permission = Name.TypeEmpty;
    }

    public static PermissionLevel ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new PermissionLevel();

        obj.Actor = Name.ReadFromBinaryReader(reader);
        obj.Permission = Name.ReadFromBinaryReader(reader);
        
        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        Actor.WriteToBinaryWriter(writer);
        Permission.WriteToBinaryWriter(writer);

        TypeObjectPool.Return(this);
    }

    public static PermissionLevel TypeEmpty = new();
}