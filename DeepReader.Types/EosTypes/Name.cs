using System.Text.Json.Serialization;
using DeepReader.Types.Fc;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(NameJsonConverter))]
public class Name : BinaryType
{
    private string? _stringVal = string.Empty;

    private ulong? _intVal;

    public string StringVal 
    { 
        get => _stringVal ??= SerializationHelper.ByteArrayToName(Binary);
        set => _stringVal = value;
    }

    public ulong IntVal
    {
        get => _intVal ??= Convert.ToUInt64(Binary);
        set => _intVal = value;
    }

    public static implicit operator string(Name value)
    {
        return value.StringVal;
    }

    public static implicit operator Name(string value)
    {
        return new Name(){ _stringVal = value};  // TODO string to ulong
    }

    public static implicit operator ulong(Name value)
    {
        return value.IntVal;
    }

    public static implicit operator Name(ulong value)
    {
        return new Name() { _intVal = value };  // TODO string to ulong
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Name item)
        {
            return false;
        }

        return Binary.Equals(item.Binary) || StringVal.Equals(item.StringVal);
    }

    public static bool operator ==(Name obj1, Name obj2)
    {
        return obj1.Binary == obj2.Binary;
    }

    public static bool operator !=(Name obj1, Name obj2)
    {
        return obj1.Binary != obj2.Binary;
    }

    public static bool operator == (string name1, Name obj2)
    {
        return name1 == obj2._stringVal;
    }

    public static bool operator !=(string name1, Name obj2)
    {
        return name1 != obj2._stringVal;
    }

    public static bool operator ==(Name name1, string name2)
    {
        return name1._stringVal == name2;
    }

    public static bool operator !=(Name name1, string name2)
    {
        return name1._stringVal != name2;
    }

    public override int GetHashCode()
    {
        return Binary.GetHashCode();
    }

    public static Name Empty => new();
}