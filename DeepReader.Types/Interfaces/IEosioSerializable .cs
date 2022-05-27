namespace DeepReader.Types.Interfaces
{
    public interface IEosioSerializable
    {

    }

    public interface IEosioSerializable<out T> : IEosioSerializable
    {
        static abstract T ReadFromBinaryReader(BinaryReader reader, bool fromPool = true);
    }
}
