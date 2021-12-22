namespace DeepReader.Types;

public class PermissionObject
{
    // Id represents the EOSIO internal id of this permission object.
    public ulong Id;// uint64 `protobuf:"varint,10,opt,name=id,proto3" json:"id,omitempty"`
    // ParentId represents the EOSIO internal id of the parent's of this permission object.
    public ulong ParentId;// uint64 `protobuf:"varint,11,opt,name=parent_id,json=parentId,proto3" json:"parent_id,omitempty"`
    // Owner is the account for which this permission was set
    public string Owner;// string `protobuf:"bytes,1,opt,name=owner,proto3" json:"owner,omitempty"`
    // Name is the permission's name this permission object is known as.
    public string Name;//                 string               `protobuf:"bytes,2,opt,name=name,proto3" json:"name,omitempty"`
    public Timestamp LastUpdated;//          *timestamp.Timestamp `protobuf:"bytes,3,opt,name=last_updated,json=lastUpdated,proto3" json:"last_updated,omitempty"`
    public Authority Authority;//            *Authority           `protobuf:"bytes,4,opt,name=authority,proto3" json:"authority,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}             `json:"-"`
    //XXX_unrecognized     []byte               `json:"-"`
    //XXX_sizecache        int32                `json:"-"`
}