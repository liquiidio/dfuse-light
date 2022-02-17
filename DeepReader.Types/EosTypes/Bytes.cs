using System.Text.Json.Serialization;
using DeepReader.Types.Fc;

namespace DeepReader.Types.EosTypes;

public class Bytes : BinaryType
{
    public Bytes()
    {

    }

    public static implicit operator Bytes(byte[] value)
    {
        return new Bytes { Binary = value };
    }

    public static implicit operator byte[](Bytes value)
    {
        return value.Binary;
    }
}

public class Bytes<T> : Bytes
{
    public T? Instance;

    [JsonIgnore]
    public bool IsDeserialized => Instance != null;

    public Bytes()
    {
        Instance = default(T);
    }

    public Bytes(T instance)
    {
        Instance = instance;
    }

    public T? GetInstance()
    {
        return Instance;
    }

    public async Task DeserializeAsync(CancellationToken cancellationToken)
    {
        if (IsDeserialized)
            return;

//        Instance = await DeepMindDeserializer.DeepMindDeserializer.DeserializeAsync<T>(Binary, cancellationToken);
    }

    public void Deserialize()
    {
        if (IsDeserialized)
            return;

//        Instance = DeepMindDeserializer.DeepMindDeserializer.Deserialize<T>(Binary);
    }

    public static implicit operator Bytes<T>(byte[] value)
    {
        return new Bytes<T> { Binary = value };
    }

    public static implicit operator byte[](Bytes<T> value)
    {
        return value.Binary;
    }
}