namespace DeepReader.Types.Interfaces
{
    public interface IEosioSerializable<T>
    {
        static abstract T ReadFromBinaryReader(BinaryReader reader);
    }
}
