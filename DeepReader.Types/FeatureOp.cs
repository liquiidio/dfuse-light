using System.Text.Json.Serialization;

namespace DeepReader.Types;

public class FeatureOp
{
    public string Kind = string.Empty;//string
    public uint ActionIndex = 0;//uint32
    public ReadOnlyMemory<char> FeatureDigest = default;//string
    public Feature Feature = new();//*Feature
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FeatureOpKind
{

}