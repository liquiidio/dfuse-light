namespace DeepReader.Types.Fc;

/// <summary>
/// libraries/fc/include/fc/io/varint.hpp
/// </summary>
public class VarUint64 : BinaryType
{
    private ulong _value;

    public static implicit operator VarUint64(ulong value)
    {
        return new VarUint64 { _value = value };
    }

    public static implicit operator ulong(VarUint64 value)
    {
        return value._value;
    }
}