using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action.hpp
/// </summary>
public class Action : ActionBase
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
        // Todo: (Haron) Type cast might be added here if we confirm it works
        // Corvin: "Yeah, Type cast here should be needed and should work. e.g.
        // var obj = ActionBase.ReadFromBinaryReader(reader);
        // ob.Data = reader.ReadActionDataBytes()
        var action = new Action()
        {
            Data = reader.ReadActionDataBytes()
        };
        return action;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}