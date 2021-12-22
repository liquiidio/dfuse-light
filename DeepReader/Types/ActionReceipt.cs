namespace DeepReader.Types;

public class ActionReceipt
{
    public string Receiver;//             string          `protobuf:"bytes,1,opt,name=receiver,proto3" json:"receiver,omitempty"`
    public string Digest;//               string          `protobuf:"bytes,2,opt,name=digest,proto3" json:"digest,omitempty"`
    public ulong GlobalSequence;//       uint64          `protobuf:"varint,3,opt,name=global_sequence,json=globalSequence,proto3" json:"global_sequence,omitempty"`
    public AuthSequence[] AuthSequence;//         []*AuthSequence `protobuf:"bytes,4,rep,name=auth_sequence,json=authSequence,proto3" json:"auth_sequence,omitempty"`
    public ulong RecvSequence;//         uint64          `protobuf:"varint,5,opt,name=recv_sequence,json=recvSequence,proto3" json:"recv_sequence,omitempty"`
    public ulong CodeSequence;//         uint64          `protobuf:"varint,6,opt,name=code_sequence,json=codeSequence,proto3" json:"code_sequence,omitempty"`
    public ulong AbiSequence;//          uint64          `protobuf:"varint,7,opt,name=abi_sequence,json=abiSequence,proto3" json:"abi_sequence,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}        `json:"-"`
    //XXX_unrecognized     []byte          `json:"-"`
    //XXX_sizecache        int32           `json:"-"`
}