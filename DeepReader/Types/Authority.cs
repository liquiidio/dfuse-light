namespace DeepReader.Types;

public class Authority
{
    public uint Threshold;//            uint32                   `protobuf:"varint,1,opt,name=threshold,proto3" json:"threshold,omitempty"`
    public KeyWeight[] Keys;//                 []*KeyWeight             `protobuf:"bytes,2,rep,name=keys,proto3" json:"keys,omitempty"`
    public PermissionLevelWeight[] Accounts;//             []*PermissionLevelWeight `protobuf:"bytes,3,rep,name=accounts,proto3" json:"accounts,omitempty"`
    public WaitWeight[] Waits;//                []*WaitWeight            `protobuf:"bytes,4,rep,name=waits,proto3" json:"waits,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}                 `json:"-"`
    //XXX_unrecognized     []byte                   `json:"-"`
    //XXX_sizecache        int32                    `json:"-"`
}