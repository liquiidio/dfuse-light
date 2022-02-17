namespace DeepReader.Types;

public class FeatureOp
{
    public string Kind = string.Empty;//string
    public uint ActionIndex = 0;//uint32
    public string FeatureDigest = string.Empty;//string
    public Feature Feature = new();//*Feature
}