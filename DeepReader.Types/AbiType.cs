using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

[Serializable]
public class Abi : IEosioSerializable<Abi>
{
    public string Version = string.Empty;
    public AbiType[] AbiTypes = Array.Empty<AbiType>();
    public AbiStruct[] AbiStructs = Array.Empty<AbiStruct>();
    public AbiAction[] AbiActions = Array.Empty<AbiAction>();
    public AbiTable[] AbiTables = Array.Empty<AbiTable>();
    public static Abi ReadFromBinaryReader(BinaryReader reader)
    {
        var abi = new Abi()
        {
            Version = reader.ReadString()
        };

        abi.AbiTypes = new AbiType[reader.Read7BitEncodedInt()];
        for (int i = 0; i < abi.AbiTypes.Length; i++)
        {
            abi.AbiTypes[i] = AbiType.ReadFromBinaryReader(reader);
        }

        abi.AbiStructs = new AbiStruct[reader.Read7BitEncodedInt()];
        for (int i = 0; i < abi.AbiStructs.Length; i++)
        {
            abi.AbiStructs[i] = AbiStruct.ReadFromBinaryReader(reader);
        }

        abi.AbiActions = new AbiAction[reader.Read7BitEncodedInt()];
        for (int i = 0; i < abi.AbiActions.Length; i++)
        {
            abi.AbiActions[i] = AbiAction.ReadFromBinaryReader(reader);
        }

        abi.AbiTables = new AbiTable[reader.Read7BitEncodedInt()];
        for (int i = 0; i < abi.AbiTables.Length; i++)
        {
            abi.AbiTables[i] = AbiTable.ReadFromBinaryReader(reader);
        }

        return abi;
    }
}

[Serializable]
public class AbiType : IEosioSerializable<AbiType>
{
    [JsonPropertyName("new_type_name")]
    public string NewTypeName = string.Empty;

    [JsonPropertyName("type")]
    public string Type = string.Empty;

    public static AbiType ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class AbiStruct : IEosioSerializable<AbiStruct>
{
    [JsonPropertyName("name")]
    public string Name = string.Empty;

    [JsonPropertyName("base")]
    public string Base = string.Empty;

    [JsonPropertyName("fields")]
    public AbiField[] Fields = Array.Empty<AbiField>();

    public static AbiStruct ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class AbiField : IEosioSerializable<AbiField>
{
    [JsonPropertyName("name")]
    public string Name = string.Empty;

    [JsonPropertyName("type")]
    public string Type = string.Empty;

    public static AbiField ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class AbiAction : IEosioSerializable<AbiAction>
{
    [JsonPropertyName("name")]
    public Name Name = Name.Empty;

    [JsonPropertyName("type")]
    public string Type = string.Empty;

    [JsonPropertyName("ricardian_contract")]
    public string RicardianContract = string.Empty;

    public static AbiAction ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class AbiTable : IEosioSerializable<AbiTable>
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

    public static AbiTable ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }
}