namespace DeepReader.Types;

public class AccountRAMDelta
{
    public string Account;//              string   `protobuf:"bytes,1,opt,name=account,proto3" json:"account,omitempty"`
    public long Delta;//                int64    `protobuf:"varint,2,opt,name=delta,proto3" json:"delta,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}