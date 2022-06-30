namespace DeepReader.Types.Interfaces
{
    public interface IFasterSerializable<T>
    {
        static abstract T ReadFromFaster(BinaryReader reader, bool fromPool = true);
        void WriteToFaster(BinaryWriter writer);
    }
}