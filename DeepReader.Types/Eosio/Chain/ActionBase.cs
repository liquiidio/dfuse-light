using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action.hpp
/// </summary>
public class ActionBase
{

    // abi-field-name: account ,abi-field-type: name
    [JsonPropertyName("account")]
    public Name Account { get; set; } = Name.Empty;

    // abi-field-name: name ,abi-field-type: name
    [JsonPropertyName("name")]
    public Name Name { get; set; } = Name.Empty;

    // abi-field-name: authorization ,abi-field-type: permission_level[]
    [JsonPropertyName("authorization")]
    public PermissionLevel[] Authorization { get; set; } = Array.Empty<PermissionLevel>();

    public ActionBase()
    {

    }

    public ActionBase(Name account, Name name, PermissionLevel[] authorization)
    {
        this.Account = account;
        this.Name = name;
        this.Authorization = authorization;
    }
}