using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepReader.EosTypes
{
    [Serializable]
    public class Abi
    {
        public string Version;
        public AbiType[] AbiTypes;
        public AbiStruct[] AbiStructs;
        public AbiAction[] AbiActions;
        public AbiTable[] AbiTables;
    }

    [Serializable]
    public class AbiType
    {
        [JsonPropertyName("new_type_name")]
        public string NewTypeName;

        [JsonPropertyName("type")]
        public string Type;
    }

    [Serializable]
    public class AbiStruct
    {
        [JsonPropertyName("name")]
        public string Name;

        [JsonPropertyName("base")]
        public string Base;

        [JsonPropertyName("fields")]
        public AbiField[] Fields;
    }

    [Serializable]
    public class AbiField
    {
        [JsonPropertyName("name")]
        public string Name;

        [JsonPropertyName("type")]
        public string Type;
    }

    [Serializable]
    public class AbiAction
    {
        [JsonPropertyName("name")]
        public Name Name;

        [JsonPropertyName("type")]
        public string Type;

        [JsonPropertyName("ricardian_contract")]
        public string RicardianContract;
    }

    [Serializable]
    public class AbiTable
    {
        [JsonPropertyName("name")]
        public Name Name;

        [JsonPropertyName("index_type")]
        public string IndexType;

        [JsonPropertyName("key_names")]
        public string[] KeyNames;

        [JsonPropertyName("key_types")]
        public string[] KeyTypes;

        [JsonPropertyName("type")]
        public string Type;
    }
}
