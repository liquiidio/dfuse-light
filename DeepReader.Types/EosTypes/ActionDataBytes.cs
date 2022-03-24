namespace DeepReader.Types.EosTypes;

/// <summary>
/// Wraps Action-bytes (byte-array) to be Abi-deserialized to specific Type
/// </summary>
public class ActionDataBytes : Bytes<object>
{
    public ActionDataBytes()
    {

    }

    public ActionDataBytes(Bytes bytes)
    {
        this.Binary = bytes.Binary;
    }

    public static implicit operator ActionDataBytes(byte[] value)
    {
        return new ActionDataBytes(value);
    }

    public static implicit operator byte[](ActionDataBytes value)
    {
        return value.Binary;
    }

    public async Task DeserializeAsync(Type targetType, CancellationToken cancellationToken)
    {
//        Instance = await DeepMindDeserializer.DeepMindDeserializer.DeserializeAsync(Binary, targetType, cancellationToken);
    }

    public void Deserialize(Type targetType)
    {
//        Instance = DeepMindDeserializer.DeepMindDeserializer.Deserialize(Binary, targetType);

    }
}