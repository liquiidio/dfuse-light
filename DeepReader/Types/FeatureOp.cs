namespace DeepReader.Types;

public class FeatureOp
{
    public string Kind;//                 string   `protobuf:"bytes,1,opt,name=kind,proto3" json:"kind,omitempty"`
    public uint ActionIndex;//          uint32   `protobuf:"varint,2,opt,name=action_index,json=actionIndex,proto3" json:"action_index,omitempty"`
    public string FeatureDigest;//        string   `protobuf:"bytes,3,opt,name=feature_digest,json=featureDigest,proto3" json:"feature_digest,omitempty"`
    public Feature Feature;//              *Feature `protobuf:"bytes,4,opt,name=feature,proto3" json:"feature,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}