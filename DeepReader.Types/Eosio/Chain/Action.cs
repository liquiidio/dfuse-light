using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action.hpp
/// </summary>
public class Action : ActionBase, IEosioSerializable<Action>
{
    // abi-field-name: data ,abi-field-type: bytes
    [JsonPropertyName("data")]
    [SortOrder(4)]
    public ActionDataBytes Data = new ();

    public Action()
    {
        Data = new ActionDataBytes();
    }

    public Action(Name account, Name name, PermissionLevel[] authorization, byte[] data) : base(account, name, authorization)
    {
        this.Data = data;
    }

    public new static Action ReadFromBinaryReader(BinaryReader reader)
    {
        var action = new Action()
        {
            Account = reader.ReadName(),
            Name = reader.ReadName()
        };

        action.Authorization = new PermissionLevel[reader.Read7BitEncodedInt()];
        for (int i = 0; i < action.Authorization.Length; i++)
        {
            action.Authorization[i] = PermissionLevel.ReadFromBinaryReader(reader);
        }

        action.Data = reader.ReadActionDataBytes();
        return action;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}