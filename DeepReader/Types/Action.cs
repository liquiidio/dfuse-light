using DeepReader.EosTypes;
using System.Text.Json.Serialization;

namespace DeepReader.Types;


[Serializable()]
public class Action
{
    public Action()
    {
    }

    public Action(Name account, Name name, PermissionLevel[] authorization, byte[] data)
    {
        this.Account = account;
        this.Name = name;
        this.Authorization = authorization;
        this.Data = data;
    }

    // abi-field-name: account ,abi-field-type: name
    [JsonPropertyName("account")]
    public Name Account;

    // abi-field-name: name ,abi-field-type: name
    [JsonPropertyName("name")]
    public Name Name;

    // abi-field-name: authorization ,abi-field-type: permission_level[]
    [JsonPropertyName("authorization")]
    public PermissionLevel[] Authorization;

    // abi-field-name: data ,abi-field-type: bytes
    [JsonPropertyName("data")]
    public ActionBytes Data;
}

public class Bytes<T> : Bytes
{
    public T Instance;

    [JsonIgnore]
    public bool IsDeserialized => Instance != null;

    public Bytes()
    {

    }

    public Bytes(T instance)
    {
        Instance = instance;
    }

    public T GetInstance()
    {
        return Instance;
    }

    public async Task DeserializeAsync(CancellationToken cancellationToken)
    {
        if (IsDeserialized)
            return;

        Instance = await Deserializer.Deserializer.DeserializeAsync<T>(_value, cancellationToken);
    }

    public void Deserialize()
    {
        if (IsDeserialized)
            return;

        Instance = Deserializer.Deserializer.Deserialize<T>(_value);
    }

    public static implicit operator Bytes<T>(byte[] value)
    {
        return new() { _value = value };
    }

    public static implicit operator byte[](Bytes<T> value)
    {
        return value._value;
    }
}

public class ActionBytes : Bytes<object>
{
    public ActionBytes()
    {

    }

    public ActionBytes(Bytes bytes)
    {
        this._value = bytes._value;
    }

    public static implicit operator ActionBytes(byte[] value)
    {
        return new(value);
    }

    public static implicit operator byte[](ActionBytes value)
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