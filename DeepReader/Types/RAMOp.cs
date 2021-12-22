namespace DeepReader.Types;

public class RAMOp
{
    public RAMOp_Operation Operation;//   RAMOp_Operation `protobuf:"varint,1,opt,name=operation,proto3,enum=dfuse.eosio.codec.v1.RAMOp_Operation" json:"operation,omitempty"`
    public uint ActionIndex;// uint32          `protobuf:"varint,2,opt,name=action_index,json=actionIndex,proto3" json:"action_index,omitempty"`
    public string Payer;//       string          `protobuf:"bytes,3,opt,name=payer,proto3" json:"payer,omitempty"`
    public long Delta;//       int64           `protobuf:"varint,4,opt,name=delta,proto3" json:"delta,omitempty"`
    public ulong Usage;//       uint64          `protobuf:"varint,5,opt,name=usage,proto3" json:"usage,omitempty"`
    // To eventually replace `operation`.
    public RAMOp_Namespace Namespace;//            RAMOp_Namespace `protobuf:"varint,6,opt,name=namespace,proto3,enum=dfuse.eosio.codec.v1.RAMOp_Namespace" json:"namespace,omitempty"`
    public string UniqueKey;//            string          `protobuf:"bytes,8,opt,name=unique_key,json=uniqueKey,proto3" json:"unique_key,omitempty"`
    public RAMOp_Action Action;//               RAMOp_Action    `protobuf:"varint,7,opt,name=action,proto3,enum=dfuse.eosio.codec.v1.RAMOp_Action" json:"action,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}        `json:"-"`
    //XXX_unrecognized     []byte          `json:"-"`
    //XXX_sizecache        int32           `json:"-"`
}