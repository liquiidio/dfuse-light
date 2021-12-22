namespace DeepReader.Types;

public class Feature
{
    public string FeatureDigest;//          string                  `protobuf:"bytes,1,opt,name=feature_digest,json=featureDigest,proto3" json:"feature_digest,omitempty"`
    public SubjectiveRestrictions SubjectiveRestrictions;// *SubjectiveRestrictions `protobuf:"bytes,2,opt,name=subjective_restrictions,json=subjectiveRestrictions,proto3" json:"subjective_restrictions,omitempty"`
    public string DescriptionDigest;//      string                  `protobuf:"bytes,3,opt,name=description_digest,json=descriptionDigest,proto3" json:"description_digest,omitempty"`
    public string[] Dependencies;//           []string                `protobuf:"bytes,4,rep,name=dependencies,proto3" json:"dependencies,omitempty"`
    public string ProtocolFeatureType;//    string                  `protobuf:"bytes,5,opt,name=protocol_feature_type,json=protocolFeatureType,proto3" json:"protocol_feature_type,omitempty"`
    public Specification[] Specification;//          []*Specification        `protobuf:"bytes,6,rep,name=specification,proto3" json:"specification,omitempty"`
    //XXX_NoUnkeyedLiteral   struct{}                `json:"-"`
    //XXX_unrecognized       []byte                  `json:"-"`
    //XXX_sizecache          int32                   `json:"-"`
}