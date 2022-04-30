﻿using System.Text.Json.Serialization;
using DeepReader.Types.Fc;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;
using DeepReader.Types.Other;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(NameJsonConverter))]
public class Name : BinaryType
{
    private string? _stringVal;

    private ulong? _intVal;

    public Name(ulong intVal, byte[] binary)
    {
        _intVal = intVal;
        Binary = binary;
    }

    public Name(ulong intVal, string stringVal, byte[] binary)
    {
        _intVal = intVal;
        _stringVal = stringVal;
        Binary = binary;
    }

    public string StringVal 
    { 
        get => _stringVal ??= SerializationHelper.ByteArrayToNameString(Binary);
        set => _stringVal = value;
    }

    public ulong IntVal
    {
        get => _intVal ??= BitConverter.ToUInt64(Binary);
        set => _intVal = value;
    }

    public static implicit operator string(Name value)
    {
        return value.StringVal;
    }

    public static implicit operator ulong(Name value)
    {
        return value.IntVal;
    }

    public static implicit operator Name(string value)
    {
        return new Name(0, value, Array.Empty<byte>()); // TODO ?
    }

    public override bool Equals(object? obj)
    {
        if (obj is ulong intVal)
        {
            return _intVal == intVal;
        }
        if (obj is not Name item)
        {
            return false;
        }
        return _intVal == item._intVal;
    }

    public static bool operator ==(Name obj1, Name obj2)
    {
        return obj1._intVal == obj2._intVal;
    }

    public static bool operator !=(Name obj1, Name obj2)
    {
        return obj1._intVal != obj2._intVal;
    }

    //public static bool operator == (string name1, Name obj2)
    //{
    //    return name1 == obj2._stringVal;
    //}

    //public static bool operator !=(string name1, Name obj2)
    //{
    //    return name1 != obj2._stringVal;
    //}

    //public static bool operator ==(Name name1, string name2)
    //{
    //    return name1._stringVal == name2;
    //}

    //public static bool operator !=(Name name1, string name2)
    //{
    //    return name1._stringVal != name2;
    //}

    public override int GetHashCode()
    {
        return Binary.GetHashCode();
    }

    public static Name Empty => NameCache.GetOrCreate(0);

    public static Name Wildcard = new(0, "*", Array.Empty<byte>());
}