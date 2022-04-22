using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action.hpp
/// </summary>
public class ActionBase
{

    // abi-field-name: account ,abi-field-type: name
    [JsonPropertyName("account")]
    [SortOrder(1)]
    public Name Account = Name.Empty;

    // abi-field-name: name ,abi-field-type: name
    [JsonPropertyName("name")]
    [SortOrder(2)]
    public Name Name = Name.Empty;

    // abi-field-name: authorization ,abi-field-type: permission_level[]
    [JsonPropertyName("authorization")]
    [SortOrder(3)]
    public PermissionLevel[] Authorization = Array.Empty<PermissionLevel>();

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