using System.Text.Json.Serialization;
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
    public ActionDataBytes Data { get; set; }

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

        var length = reader.Read7BitEncodedInt();
        Data = reader.ReadBytes(length);
    }

    public static Action ReadFromBinaryReader(BinaryReader reader)
    {
        return new Action(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.WriteName(Account);
        writer.WriteName(Name);

        writer.Write7BitEncodedInt(Authorization.Length);
        foreach (var auth in Authorization)
        {
            auth.WriteToBinaryWriter(writer);
        }

        writer.Write7BitEncodedInt(Data.Binary.Length);
        writer.Write(Data.Binary);
    }
}