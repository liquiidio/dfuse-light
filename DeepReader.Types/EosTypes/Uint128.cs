using DeepReader.Types.Fc;

namespace DeepReader.Types.EosTypes;

public class Uint128 : BinaryType
{
    public static implicit operator Uint128(byte[] binary)
    {
        return new Uint128 { Binary = binary };
    }

    public static implicit operator byte[](Uint128 value)
    {
        return value.Binary;
    }
}