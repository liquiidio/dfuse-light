namespace DeepReader.Types;

public class Feature
{
    public string FeatureDigest = string.Empty;//string
    public SubjectiveRestrictions SubjectiveRestrictions = new();//*SubjectiveRestrictions
    public string DescriptionDigest = string.Empty;//string
    public string[] Dependencies = Array.Empty<string>();//[]string
    public string ProtocolFeatureType = string.Empty;//string
    public Specification[] Specification = Array.Empty<Specification>();//[]*Specification
}