using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action.hpp
/// </summary>
public sealed class Action : ActionBase, IEosioSerializable<Action>
{
    public ulong Id { get; set; }

    // abi-field-name: data ,abi-field-type: bytes
    [JsonPropertyName("data")]
    public ActionDataBytes Data { get; set; }

    public Action()
    {
        Data = new ActionDataBytes();
    }

    public Action(Name account, Name name, List<PermissionLevel> authorization, byte[] data) : base(account, name, authorization)
    {
        this.Data = data;
    }

    public Action(BinaryReader reader)
    {
        Account = Name.ReadFromBinaryReader(reader);
        Name = Name.ReadFromBinaryReader(reader);

        var permissionLevelCount = reader.Read7BitEncodedInt();
        Authorization = new List<PermissionLevel>(permissionLevelCount);
        for (int i = 0; i < permissionLevelCount; i++)
        {
            Authorization.Add(PermissionLevel.ReadFromBinaryReader(reader));
        }

        var length = reader.Read7BitEncodedInt();
        Data = reader.ReadBytes(length);
    }

    public static Action ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        return new Action(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        Account.WriteToBinaryWriter(writer);
        Name.WriteToBinaryWriter(writer);

        writer.Write7BitEncodedInt(Authorization.Count);
        foreach (var auth in Authorization)
        {
            auth.WriteToBinaryWriter(writer);
        }

        writer.Write7BitEncodedInt(Data.Binary.Length);
        writer.Write(Data.Binary);
    }
}