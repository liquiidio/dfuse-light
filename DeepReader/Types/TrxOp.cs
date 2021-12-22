namespace DeepReader.Types;

public class TrxOp
{
    public TrxOp_Operation Operation;//            TrxOp_Operation    `protobuf:"varint,1,opt,name=operation,proto3,enum=dfuse.eosio.codec.v1.TrxOp_Operation" json:"operation,omitempty"`
    public string Name;//                 string             `protobuf:"bytes,2,opt,name=name,proto3" json:"name,omitempty"`
    public string TransactionId;//        string             `protobuf:"bytes,3,opt,name=transaction_id,json=transactionId,proto3" json:"transaction_id,omitempty"`
    public SignedTransaction Transaction;//          *SignedTransaction `protobuf:"bytes,4,opt,name=transaction,proto3" json:"transaction,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}           `json:"-"`
    //XXX_unrecognized     []byte             `json:"-"`
    //XXX_sizecache        int32              `json:"-"`
}