using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action.hpp
/// </summary>
public sealed class Action : ActionBase, IEosioSerializable<Action>, IFasterSerializable<Action>
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

    public Action(IBufferReader reader)
    {
        Account = Name.ReadFromBinaryReader(reader);
        Name = Name.ReadFromBinaryReader(reader);

        Authorization = new PermissionLevel[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Authorization.Length; i++)
        {
            Authorization[i] = PermissionLevel.ReadFromBinaryReader(reader);
        }

        var length = reader.Read7BitEncodedInt();
        Data = reader.ReadBytes(length);
    }

    public static Action ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new Action(reader);
    }

    public static Action ReadFromFaster(IBufferReader reader, bool fromPool = true)
    {
        var obj = new Action()
        {
            Account = Name.ReadFromFaster(reader),
            Name = Name.ReadFromFaster(reader)
        };


        obj.Authorization = new PermissionLevel[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.Authorization.Length; i++)
        {
            obj.Authorization[i] = PermissionLevel.ReadFromFaster(reader);
        }

        var length = reader.Read7BitEncodedInt();
        obj.Data = reader.ReadBytes(length);

        return obj;
    }

    public void WriteToFaster(IBufferWriter writer)
    {
        Account.WriteToFaster(writer);
        Name.WriteToFaster(writer);

        writer.Write7BitEncodedInt(Authorization.Length);
        foreach (var auth in Authorization)
        {
            auth.WriteToFaster(writer);
        }

        writer.Write7BitEncodedInt(Data.Binary.Length);
        writer.Write(Data.Binary);
    }
}