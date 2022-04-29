namespace DeepReader.Benchmarks;

public interface IParent<T>
{
    static abstract T ReadFromBinaryReader();
}