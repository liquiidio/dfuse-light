using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;

namespace DeepReader.Types.EosTypes;

public sealed class Float128 : IEosioSerializable<Float128>, IFasterSerializable<Float128>
{
    private const int Float128ByteLength = 16;

    private byte[] _binary = Array.Empty<byte>();

    public Float128()
    {

    }

    public Float128(byte[] bytes)
    {
        _binary = bytes;
    }

    public static Float128 ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new Float128(reader.ReadBytes(Float128ByteLength));
    }

    public static Float128 ReadFromFaster(IBufferReader reader, bool fromPool = true)
    {
        return new Float128(reader.ReadBytes(Float128ByteLength));
    }

    public void WriteToFaster(IBufferWriter writer)
    {
        writer.Write(_binary);
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