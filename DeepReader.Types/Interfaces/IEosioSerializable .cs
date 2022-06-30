namespace DeepReader.Types.Interfaces
{
    public interface IEosioSerializable
    {

    }

    public interface IEosioSerializable<out T> : IEosioSerializable
    {
        static abstract T ReadFromBinaryReader(IBufferReader reader, bool fromPool = true);
    }
}