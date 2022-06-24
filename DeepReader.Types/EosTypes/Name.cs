using System.Text.Json.Serialization;
using DeepReader.Types.Fc;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;
using DeepReader.Types.Other;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(NameJsonConverter))]
public sealed class Name : BinaryType, IEosioSerializable<Name>, IFasterSerializable<Name>
{
    private const int NameByteLength = 8;

    private string? _stringVal;

    private ulong? _intVal;

    public Name()
    {

    }

    //public Name(ulong intVal, byte[] binary)
    //{
    //    _intVal = intVal;
    //    Binary = binary;
    //}

    //public Name(ulong intVal, string stringVal, byte[] binary)
    //{
    //    _intVal = intVal;
    //    _stringVal = stringVal;
    //    Binary = binary;
    //}
    public static Name ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return NameCache.GetOrCreate(reader.ReadUInt64());
    }

    public static Name ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        return NameCache.GetOrCreate(reader.ReadUInt64());
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        writer.Write(Binary);
    }

    public void Set(ulong intVal, byte[] binary)
    {
        _intVal = intVal;
        Binary = binary;
    }

    public void Set(ulong intVal, string stringVal, byte[] binary)
    {
        _intVal = intVal;
        _stringVal = stringVal;
        Binary = binary;
    }

    public void Clear()
    {
        _intVal = 0;
        _stringVal = null;
        Binary = Array.Empty<byte>();
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
        var name = new Name();
        name.Set(0, value, Array.Empty<byte>());
        return name;
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

    public static readonly Name TypeEmpty = NameCache.GetOrCreate(0);

    public static readonly Name TypeWildcard = new()
    {
        _intVal = 0,
        _stringVal = "*",
        Binary = new byte[8]
    };
}