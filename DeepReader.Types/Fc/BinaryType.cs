using System.Text.Json.Serialization;

namespace DeepReader.Types.Fc;

public abstract class BinaryType
{
    [JsonIgnore]
    public byte[] Binary = Array.Empty<byte>();
}