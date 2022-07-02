using DeepReader.Types.Infrastructure.BinaryReaders;

namespace DeepReader.Types.Interfaces
{
    public interface IEosioSerializable<out T>
    {
        static abstract T ReadFromBinaryReader(IBufferReader reader, bool fromPool = true);
    }
}