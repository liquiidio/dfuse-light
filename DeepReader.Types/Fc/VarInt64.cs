namespace DeepReader.Types.Fc;

/// <summary>
/// libraries/fc/include/fc/io/varint.hpp
/// </summary>
public class VarInt64 : BinaryType
{
    private long _value;

    public static implicit operator VarInt64(long value)
    {
        return new VarInt64 { _value = value };
    }

    public static implicit operator long(VarInt64 value)
    {
        return value._value;
    }
}