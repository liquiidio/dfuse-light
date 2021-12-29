using System.Text.Json.Serialization;

namespace DeepReader.Types;

[Serializable()]
public class Extension
{
    // abi-field-name: type ,abi-field-type: uint16
    [JsonPropertyName("type")]
    public ushort Type;

    // abi-field-name: data ,abi-field-type: bytes
    [JsonPropertyName("data")]
    public byte[] Data;
//    public Bytes Data;

    public Extension(ushort type, byte[] data)
    {
        this.Type = type;
        this.Data = data;
    }

    public Extension()
    {
    }
}