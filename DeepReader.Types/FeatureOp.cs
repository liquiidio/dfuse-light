using System.Text.Json.Serialization;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types;

[JsonConverter(typeof(FeatureOpJsonConverter))]
public sealed class FeatureOp
{
    public FeatureOpKind Kind = FeatureOpKind.UNKNOWN;//string
    public uint ActionIndex = 0;//uint32
    [JsonIgnore]
    public ReadOnlyMemory<char> FeatureDigest = default;//string
    public Feature Feature = new();//*Feature
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FeatureOpKind : byte
{
    UNKNOWN,
    ACTIVATE,
    PRE_ACTIVATE,
}