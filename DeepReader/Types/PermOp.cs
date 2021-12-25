namespace DeepReader.Types;

public class PermOp
{
    public PermOpOperation Operation;//            PermOp_Operation  `protobuf:"varint,1,opt,name=operation,proto3,enum=dfuse.eosio.codec.v1.PermOp_Operation" json:"operation,omitempty"`
    public uint ActionIndex;//          uint32            `protobuf:"varint,2,opt,name=action_index,json=actionIndex,proto3" json:"action_index,omitempty"`
    public PermissionObject OldPerm;//              *PermissionObject `protobuf:"bytes,8,opt,name=old_perm,json=oldPerm,proto3" json:"old_perm,omitempty"`
    public PermissionObject NewPerm;//              *PermissionObject `protobuf:"bytes,9,opt,name=new_perm,json=newPerm,proto3" json:"new_perm,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}          `json:"-"`
    //XXX_unrecognized     []byte            `json:"-"`
    //XXX_sizecache        int32             `json:"-"`
}