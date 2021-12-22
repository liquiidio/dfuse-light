namespace DeepReader;

public static class Deserializer {
    public static T Deserialize<T>(byte[] bytes) where T : new()
    {
        return new T();
    }
}