using DeepReader.Types.Fc;

namespace DeepReader.Types.EosTypes;

public class Checksum160 : BinaryType
{
    private string _value = string.Empty;

    public static implicit operator Checksum160(string value)
    {
        return new Checksum160 {_value = value};
    }

    public static implicit operator string(Checksum160 value)
    {
        return value._value;
    }

    public static implicit operator Checksum160(byte[] binary)
    {
        return new Checksum160 { Binary = binary };
    }

    public static implicit operator byte[](Checksum160 value)
    {
        return value.Binary;
    }

    public static Checksum160 Empty => new();
}