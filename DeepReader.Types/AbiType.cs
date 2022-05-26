using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types;

[Serializable]
public sealed class Abi : IEosioSerializable<Abi>
{
    public string Version;
    public AbiType[] AbiTypes;
    public AbiStruct[] AbiStructs;
    public AbiAction[] AbiActions;
    public AbiTable[] AbiTables;

    public Abi(BinaryReader reader)
    {
        Version = reader.ReadString();

        AbiTypes = new AbiType[reader.Read7BitEncodedInt()];
        for (var i = 0; i < AbiTypes.Length; i++)
        {
            AbiTypes[i] = AbiType.ReadFromBinaryReader(reader);
        }

        AbiStructs = new AbiStruct[reader.Read7BitEncodedInt()];
        for (var i = 0; i < AbiStructs.Length; i++)
        {
            AbiStructs[i] = AbiStruct.ReadFromBinaryReader(reader);
        }

        AbiActions = new AbiAction[reader.Read7BitEncodedInt()];
        for (var i = 0; i < AbiActions.Length; i++)
        {
            AbiActions[i] = AbiAction.ReadFromBinaryReader(reader);
        }

        AbiTables = new AbiTable[reader.Read7BitEncodedInt()];
        for (var i = 0; i < AbiTables.Length; i++)
        {
            AbiTables[i] = AbiTable.ReadFromBinaryReader(reader);
        }
    }

    public static Abi ReadFromBinaryReader(BinaryReader reader)
    {
        return new Abi(reader);
    }
}

[Serializable]
public sealed class AbiType : IEosioSerializable<AbiType>
{
    [JsonPropertyName("new_type_name")]
    public string NewTypeName;

    [JsonPropertyName("type")]
    public string Type;

    public AbiType(BinaryReader reader)
    {
        NewTypeName = reader.ReadString();
        Type = reader.ReadString();
    }

    public static AbiType ReadFromBinaryReader(BinaryReader reader)
    {
        return new AbiType(reader);
    }
}

[Serializable]
public sealed class AbiStruct : IEosioSerializable<AbiStruct>
{
    [JsonPropertyName("name")]
    public string Name;

    [JsonPropertyName("base")]
    public string Base;

    [JsonPropertyName("fields")]
    public AbiField[] Fields;

    public AbiStruct(string name, List<AbiField> fields)
    {
        Name = name;
        Base = string.Empty;
        Fields = fields.ToArray();
    }

    public AbiStruct(BinaryReader reader)
    {
        Name = reader.ReadString();
        Base = reader.ReadString();

        Fields = new AbiField[reader.Read7BitEncodedInt()];
        for (var i = 0; i < Fields.Length; i++)
        {
            Fields[i] = AbiField.ReadFromBinaryReader(reader);
        }
    }

    public static AbiStruct ReadFromBinaryReader(BinaryReader reader)
    {
        return new AbiStruct(reader);
    }
}

[Serializable]
public sealed class AbiField : IEosioSerializable<AbiField>
{
    [JsonPropertyName("name")]
    public string Name;

    [JsonPropertyName("type")]
    public string Type;

    public AbiField(BinaryReader reader)
    {
        Name = reader.ReadString();
        Type = reader.ReadString();
    }

    public static AbiField ReadFromBinaryReader(BinaryReader reader)
    {
        return new AbiField(reader);
    }
}

[Serializable]
public sealed class AbiAction : IEosioSerializable<AbiAction>
{
    [JsonPropertyName("name")]
    public Name Name;

    [JsonPropertyName("type")]
    public string Type;

    [JsonPropertyName("ricardian_contract")]
    public string RicardianContract;

    public AbiAction(BinaryReader reader)
    {
        Name = reader.ReadName();
        Type = reader.ReadString();
        RicardianContract = reader.ReadString();
    }

    public static AbiAction ReadFromBinaryReader(BinaryReader reader)
    {
        return new AbiAction(reader);
    }
}

[Serializable]
public sealed class AbiTable : IEosioSerializable<AbiTable>
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

    public AbiTable(BinaryReader reader)
    {

        Name = reader.ReadName();
        IndexType = reader.ReadString();

        KeyNames = new string[reader.Read7BitEncodedInt()];
        for (var i = 0; i < KeyNames.Length; i++)
        {
            KeyNames[i] = reader.ReadString();
        }

        KeyTypes = new string[reader.Read7BitEncodedInt()];
        for (var i = 0; i < KeyTypes.Length; i++)
        {
            KeyTypes[i] = reader.ReadString();
        }

        Type = reader.ReadString();
    }

    public static AbiTable ReadFromBinaryReader(BinaryReader reader)
    {
        return new AbiTable(reader);
    }
}