using DeepReader.Types.Fc;

namespace DeepReader.Types.EosTypes;

public class Int128 : BinaryType
{
    public static implicit operator Int128(byte[] binary)
    {
        return new Int128 { Binary = binary };
    }

    public static implicit operator byte[](Int128 value)
    {
        return value.Binary;
    }
}