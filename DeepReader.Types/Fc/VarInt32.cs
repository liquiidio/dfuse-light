namespace DeepReader.Types.Fc;

/// <summary>
/// libraries/fc/include/fc/io/varint.hpp
/// </summary>
public class VarInt32 : BinaryType
{
    private int _value;

    public static implicit operator VarInt32(int value)
    {
        return new VarInt32 { _value = value };
    }

    public static implicit operator int(VarInt32 value)
    {
        return value._value;
    }
}