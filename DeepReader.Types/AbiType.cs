﻿using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Interfaces;

namespace DeepReader.Types;

[Serializable]
public sealed class Abi : IEosioSerializable<Abi>
{
    public string Version;
    public AbiType[] AbiTypes;
    public AbiStruct[] AbiStructs;
    public AbiAction[] AbiActions;
    public AbiTable[] AbiTables;

    public Abi() { }

    public Abi(IBufferReader reader)
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

    public static Abi ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
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

    public AbiType(IBufferReader reader)
    {
        NewTypeName = reader.ReadString();
        Type = reader.ReadString();
    }

    public static AbiType ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
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

    public AbiStruct(IBufferReader reader)
    {
        Name = reader.ReadString();
        Base = reader.ReadString();

        Fields = new AbiField[reader.Read7BitEncodedInt()];
        for (var i = 0; i < Fields.Length; i++)
        {
            Fields[i] = AbiField.ReadFromBinaryReader(reader);
        }
    }

    public static AbiStruct ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
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

    public AbiField(IBufferReader reader)
    {
        Name = reader.ReadString();
        Type = reader.ReadString();
    }

    public static AbiField ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
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

    public AbiAction(IBufferReader reader)
    {
        Name = Name.ReadFromBinaryReader(reader);
        Type = reader.ReadString();
        RicardianContract = reader.ReadString();
    }

    public static AbiAction ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
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

    public AbiTable(IBufferReader reader)
    {

        Name = Name.ReadFromBinaryReader(reader);
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

    public static AbiTable ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new AbiTable(reader);
    }
}