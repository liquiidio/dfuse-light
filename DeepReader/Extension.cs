namespace DeepReader;

public class Extension
{
    public uint Type;//                 uint32   `protobuf:"varint,1,opt,name=type,proto3" json:"type,omitempty"`
    public byte[] Data;//                 []byte   `protobuf:"bytes,2,opt,name=data,proto3" json:"data,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}