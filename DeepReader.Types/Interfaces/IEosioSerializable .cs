namespace DeepReader.Types.Interfaces
{
    public interface IEosioSerializable<out T>
    {
        static abstract T ReadFromBinaryReader(BinaryReader reader);
    }
}
