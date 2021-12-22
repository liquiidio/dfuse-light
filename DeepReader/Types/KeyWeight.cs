namespace DeepReader.Types;

public class KeyWeight
{
    public string PublicKey;//            string   `protobuf:"bytes,1,opt,name=public_key,json=publicKey,proto3" json:"public_key,omitempty"`
    public uint Weight;//               uint32   `protobuf:"varint,2,opt,name=weight,proto3" json:"weight,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`   
}