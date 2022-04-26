using System.Text.Json.Serialization;

namespace DeepReader.Types.Fc;

public abstract class BinaryType
{
    [JsonIgnore]
    public byte[] Binary { get; set; } = Array.Empty<byte>();
}