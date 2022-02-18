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

    public static Action ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}