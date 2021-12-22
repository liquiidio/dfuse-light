namespace DeepReader.Types;

public class AuthSequence
{
    public string AccountName;//          string   `protobuf:"bytes,1,opt,name=account_name,json=accountName,proto3" json:"account_name,omitempty"`
    public ulong Sequence;//             uint64   `protobuf:"varint,2,opt,name=sequence,proto3" json:"sequence,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}