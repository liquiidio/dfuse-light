namespace DeepReader.Types;

public class PermissionLevelWeight
{
    public PermissionLevel Permission;//           *PermissionLevel `protobuf:"bytes,1,opt,name=permission,proto3" json:"permission,omitempty"`
    public uint Weight;//               uint32           `protobuf:"varint,2,opt,name=weight,proto3" json:"weight,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}         `json:"-"`
    //XXX_unrecognized     []byte           `json:"-"`
    //XXX_sizecache        int32            `json:"-"`
}