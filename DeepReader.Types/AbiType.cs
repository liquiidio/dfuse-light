using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

[Serializable]
public class Abi
{
    public string Version = string.Empty;
    public AbiType[] AbiTypes = Array.Empty<AbiType>();
    public AbiStruct[] AbiStructs = Array.Empty<AbiStruct>();
    public AbiAction[] AbiActions = Array.Empty<AbiAction>();
    public AbiTable[] AbiTables = Array.Empty<AbiTable>();
}

[Serializable]
public class AbiType
{
    [JsonPropertyName("new_type_name")]
    public string NewTypeName = string.Empty;

    [JsonPropertyName("type")]
    public string Type = string.Empty;
}

[Serializable]
public class AbiStruct
{
    [JsonPropertyName("name")]
    public string Name = string.Empty;

    [JsonPropertyName("base")]
    public string Base = string.Empty;

    [JsonPropertyName("fields")]
    public AbiField[] Fields = Array.Empty<AbiField>();
}

[Serializable]
public class AbiField
{
    [JsonPropertyName("name")]
    public string Name = string.Empty;

    [JsonPropertyName("type")]
    public string Type = string.Empty;
}

[Serializable]
public class AbiAction
{
    [JsonPropertyName("name")]
    public Name Name = Name.Empty;

    [JsonPropertyName("type")]
    public string Type = string.Empty;

    [JsonPropertyName("ricardian_contract")]
    public string RicardianContract = string.Empty;
}

[Serializable]
public class AbiTable
{
    [JsonPropertyName("name")]
    public Name Name = Name.Empty;

    [JsonPropertyName("index_type")]
    public string IndexType = string.Empty;

    [JsonPropertyName("key_names")]
    public string[] KeyNames = Array.Empty<string>();

    [JsonPropertyName("key_types")]
    public string[] KeyTypes = Array.Empty<string>();

    [JsonPropertyName("type")]
    public string Type = string.Empty;
}