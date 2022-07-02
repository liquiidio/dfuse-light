using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;

namespace DeepReader.Types.Interfaces
{
    public interface IFasterSerializable<out T>
    {
        static abstract T ReadFromFaster(IBufferReader reader, bool fromPool = true);
        void WriteToFaster(IBufferWriter writer);
    }
}