using System.Text.Json.Serialization;
using DeepReader.DeepMindDeserializer;
using DeepReader.Helpers;

namespace DeepReader.Types.EosTypes;

public class Bytes
{
    internal byte[] Value = Array.Empty<byte>();

    public Bytes()
    {

    }

    public static implicit operator Bytes(byte[] value)
    {
        return new Bytes { Value = value };
    }

    public static implicit operator byte[](Bytes value)
    {
        return value.Value;
    }

    public string ToJson()
    {
        return SerializationHelper.ByteArrayToHexString(Value);
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

        Instance = await Deserializer.DeserializeAsync<T>(Value, cancellationToken);
    }

    public void Deserialize()
    {
        if (IsDeserialized)
            return;

        Instance = Deserializer.Deserialize<T>(Value);
    }

    public static implicit operator Bytes<T>(byte[] value)
    {
        return new Bytes<T> { Value = value };
    }

    public static implicit operator byte[](Bytes<T> value)
    {
        return value.Value;
    }
}