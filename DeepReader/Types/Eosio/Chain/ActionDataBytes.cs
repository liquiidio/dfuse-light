using DeepReader.EosTypes;

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
        this._value = bytes._value;
    }

    public static implicit operator ActionDataBytes(byte[] value)
    {
        return new(value);
    }

    public static implicit operator byte[](ActionDataBytes value)
    {
        return value._value;
    }

    public async Task DeserializeAsync(Type targetType, CancellationToken cancellationToken)
    {
        Instance = await Deserializer.Deserializer.DeserializeAsync(_value, targetType, cancellationToken);
    }

    public void Deserialize(Type targetType)
    {
        Instance = Deserializer.Deserializer.Deserialize(_value, targetType);

    }
}