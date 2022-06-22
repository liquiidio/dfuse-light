namespace DeepReader.Types.Interfaces
{
    public interface IFasterSerializable<T>
    {
        public T Deserialize(BinaryReader reader);
        public void Serialize(BinaryWriter writer);
    }
}