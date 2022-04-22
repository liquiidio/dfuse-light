using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action.hpp
/// </summary>
public class Action : ActionBase, IEosioSerializable<Action>
{
    // abi-field-name: data ,abi-field-type: bytes
    [JsonPropertyName("data")]
    [SortOrder(4)]
    public ActionDataBytes Data;

    public Action()
    {
        Data = new ActionDataBytes();
    }

    public Action(Name account, Name name, PermissionLevel[] authorization, byte[] data) : base(account, name, authorization)
    {
        this.Data = data;
    }

    public Action(BinaryReader reader)
    {
        Account = reader.ReadName();
        Name = reader.ReadName();

        Authorization = new PermissionLevel[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Authorization.Length; i++)
        {
            Authorization[i] = PermissionLevel.ReadFromBinaryReader(reader);
        }

        Data = reader.ReadActionDataBytes();
    }

    public new static Action ReadFromBinaryReader(BinaryReader reader)
    {
        return new Action(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.WriteName(Account);
        writer.WriteName(Name);

        writer.Write(Authorization.Length);
        foreach (var auth in Authorization)
        {
            auth.WriteToBinaryWriter(writer);
        }

        writer.Write(Data.Binary.Length);
        writer.Write(Data.Binary);
    }
}