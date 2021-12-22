namespace DeepReader.Types;

public class WaitWeight
{
    public uint WaitSec;//              uint32   `protobuf:"varint,1,opt,name=wait_sec,json=waitSec,proto3" json:"wait_sec,omitempty"`
    public uint Weight;//               uint32   `protobuf:"varint,2,opt,name=weight,proto3" json:"weight,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}