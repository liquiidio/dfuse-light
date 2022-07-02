using DeepReader.Types.Fc;
using DeepReader.Types.Infrastructure.BinaryReaders;

namespace DeepReader.Types.EosTypes;

public sealed class Uint128 : BinaryType, IEosioSerializable<Uint128>
{
    private const int Uint128ByteLength = 16;

    public static implicit operator Uint128(byte[] binary)
    {
        return new Uint128 { Binary = binary };
    }

    public static implicit operator byte[](Uint128 value)
    {
        return value.Binary;
    }

    public static Uint128 ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return reader.ReadBytes(Uint128ByteLength);
    }
}