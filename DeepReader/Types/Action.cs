using DeepReader.EosTypes;
using System.Text.Json.Serialization;

namespace DeepReader.Types;


[Serializable()]
public class Action
{
    // abi-field-name: account ,abi-field-type: name
    [JsonPropertyName("account")]
    public Name Account;

    // abi-field-name: name ,abi-field-type: name
    [JsonPropertyName("name")]
    public Name Name;

    // abi-field-name: authorization ,abi-field-type: permission_level[]
    [JsonPropertyName("authorization")]
    public PermissionLevel[] Authorization;

    // abi-field-name: data ,abi-field-type: bytes
    [JsonPropertyName("data")]
    public byte [] Data;
//    public ActionBytes Data;

    public Action(Name account, Name name, PermissionLevel[] authorization, byte[] data)
    {
        this.Account = account;
        this.Name = name;
        this.Authorization = authorization;
        this.Data = data;
    }

    public Action()
    {
    }
}