using DeepReader.Types.Extensions;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Interfaces;

namespace DeepReader.Types.Fc;

/// <summary>
/// libraries/fc/include/fc/io/varint.hpp
/// </summary>
public sealed class VarUint32 : BinaryType, IEosioSerializable<VarUint32>
{
    public uint Value;

    public static implicit operator VarUint32(uint value)
    {
        return new VarUint32 { Value = value };
    }

    public static implicit operator uint(VarUint32 value)
    {
        return value.Value;
    }

    public static VarUint32 ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    { 
        return (uint)reader.Read7BitEncodedInt();
    }
}