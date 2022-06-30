using DeepReader.Types.Fc;

namespace DeepReader.Types.EosTypes;

public sealed class Int128 : BinaryType, IEosioSerializable<Int128>
{
    private const int Int128ByteLength = 16;

    public static implicit operator Int128(byte[] binary)
    {
        return new Int128 { Binary = binary };
    }

    public static implicit operator byte[](Int128 value)
    {
        return value.Binary;
    }

    public static Int128 ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return reader.ReadBytes(Int128ByteLength);
    }
}