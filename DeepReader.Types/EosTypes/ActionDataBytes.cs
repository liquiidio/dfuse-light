using System.Text.Json;

namespace DeepReader.Types.EosTypes;

/// <summary>
/// Wraps Action-bytes (byte-array) to be Abi-deserialized to specific Type
/// </summary>
public sealed class ActionDataBytes : Bytes<object>
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

    public JsonElement? Json { get; set; }
    public string Hex { get; set; } // TODO
}