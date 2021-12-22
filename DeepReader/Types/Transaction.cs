namespace DeepReader.Types;

public class Transaction
{
    public TransactionHeader Header;//               *TransactionHeader `protobuf:"bytes,1,opt,name=header,proto3" json:"header,omitempty"`
    public Action[] ContextFreeActions;//   []*Action          `protobuf:"bytes,2,rep,name=context_free_actions,json=contextFreeActions,proto3" json:"context_free_actions,omitempty"`
    public Action[] Actions;//              []*Action          `protobuf:"bytes,3,rep,name=actions,proto3" json:"actions,omitempty"`
    public Extension[] Extensions;//           []*Extension       `protobuf:"bytes,4,rep,name=extensions,proto3" json:"extensions,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}           `json:"-"`
    //XXX_unrecognized     []byte             `json:"-"`
    //XXX_sizecache        int32              `json:"-"`
}