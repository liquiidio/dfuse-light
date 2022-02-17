namespace DeepReader.Types.Fc.Crypto;

public class Signature : BinaryType
{
    private string _value = string.Empty;

    public static implicit operator Signature(string value)
    {
        return new Signature { _value = value };
    }

    public static implicit operator string(Signature value)
    {
        return value._value;
    }

    public static implicit operator Signature(byte[] binary)
    {
        return new Signature { Binary = binary };
    }

    public static implicit operator byte[](Signature value)
    {
        return value.Binary;
    }

    public static Signature Empty => new();
}