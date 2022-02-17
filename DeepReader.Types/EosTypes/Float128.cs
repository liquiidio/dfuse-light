namespace DeepReader.Types.EosTypes;

public class Float128
{
    private byte[] _binary = Array.Empty<byte>();

    public Float128()
    {

    }

    public Float128(byte[] bytes)
    {
        _binary = bytes;
    }

    public static implicit operator Float128(byte[] binary)
    {
        return new Float128(){ _binary = binary };
    }

    public static implicit operator byte[](Float128 value)
    {
        return value._binary;
    }
}