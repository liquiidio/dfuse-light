using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

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
        this.Value = bytes.Value;
    }

    public static implicit operator ActionDataBytes(byte[] value)
    {
        return new ActionDataBytes(value);
    }

    public static implicit operator byte[](ActionDataBytes value)
    {
        return value.Value;
    }

    public async Task DeserializeAsync(Type targetType, CancellationToken cancellationToken)
    {
        Instance = await Deserializer.Deserializer.DeserializeAsync(Value, targetType, cancellationToken);
    }

    public void Deserialize(Type targetType)
    {
        Instance = Deserializer.Deserializer.Deserialize(Value, targetType);

    }
}