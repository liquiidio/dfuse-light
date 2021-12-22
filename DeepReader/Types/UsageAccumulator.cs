namespace DeepReader.Types;

public class UsageAccumulator
{
    public uint LastOrdinal;//          uint32   `protobuf:"varint,1,opt,name=last_ordinal,json=lastOrdinal,proto3" json:"last_ordinal,omitempty"`
    public ulong ValueEx;//              uint64   `protobuf:"varint,2,opt,name=value_ex,json=valueEx,proto3" json:"value_ex,omitempty"`
    public ulong Consumed;//             uint64   `protobuf:"varint,3,opt,name=consumed,proto3" json:"consumed,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}