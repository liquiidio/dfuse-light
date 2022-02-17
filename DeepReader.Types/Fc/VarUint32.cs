namespace DeepReader.Types.Fc;

/// <summary>
/// libraries/fc/include/fc/io/varint.hpp
/// </summary>
public class VarUint32 : BinaryType
{
    private uint _value;

    public static implicit operator VarUint32(uint value)
    {
        return new VarUint32 { _value = value };
    }

    public static implicit operator uint(VarUint32 value)
    {
        return value._value;
    }
}