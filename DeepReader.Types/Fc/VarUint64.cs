using DeepReader.Types.Extensions;

namespace DeepReader.Types.Fc;

/// <summary>
/// libraries/fc/include/fc/io/varint.hpp
/// </summary>
public sealed class VarUint64 : BinaryType, IEosioSerializable<VarUint64>
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

    public static VarUint64 ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return (ulong)reader.Read7BitEncodedInt64();
    }
}