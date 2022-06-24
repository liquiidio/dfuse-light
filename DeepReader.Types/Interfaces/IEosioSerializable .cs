using Salar.BinaryBuffers;

namespace DeepReader.Types.Interfaces
{
    public interface IEosioSerializable
    {

    }

    public interface IEosioSerializable<out T> : IEosioSerializable
    {
        static abstract T ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true);
    }
}