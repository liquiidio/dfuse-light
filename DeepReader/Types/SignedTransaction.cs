namespace DeepReader.Types;

public class SignedTransaction
{
    public Transaction Transaction;//          *Transaction `protobuf:"bytes,1,opt,name=transaction,proto3" json:"transaction,omitempty"`
    public string[] Signatures;//           []string     `protobuf:"bytes,2,rep,name=signatures,proto3" json:"signatures,omitempty"`
    public byte[][] ContextFreeData;//      [][]byte     `protobuf:"bytes,3,rep,name=context_free_data,json=contextFreeData,proto3" json:"context_free_data,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}     `json:"-"`
    //XXX_unrecognized     []byte       `json:"-"`
    //XXX_sizecache        int32        `json:"-"`s
}