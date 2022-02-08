namespace DeepReader.Types.EosTypes;

public class Checksum160
{
    private string _value = string.Empty;

    public static implicit operator Checksum160(string value)
    {
        return new Checksum160 { _value = value };
    }

    public static implicit operator string(Checksum160 value)
    {
        return value._value;
    }

    public static Checksum160 Empty => new();
}