namespace DeepReader.Types;

public class SubjectiveRestrictions
{
    public bool Enabled;//                       bool     `protobuf:"varint,1,opt,name=enabled,proto3" json:"enabled,omitempty"`
    public bool PreactivationRequired;//         bool     `protobuf:"varint,2,opt,name=preactivation_required,json=preactivationRequired,proto3" json:"preactivation_required,omitempty"`
    public string EarliestAllowedActivationTime;// string   `protobuf:"bytes,3,opt,name=earliest_allowed_activation_time,json=earliestAllowedActivationTime,proto3" json:"earliest_allowed_activation_time,omitempty"`
    //XXX_NoUnkeyedLiteral          struct{} `json:"-"`
    //XXX_unrecognized              []byte   `json:"-"`
    //XXX_sizecache                 int32    `json:"-"`
}