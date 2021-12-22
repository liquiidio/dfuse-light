namespace DeepReader.Types;

public class DTrxOp
{
    public DTrxOp_Operation Operation;//            DTrxOp_Operation   `protobuf:"varint,1,opt,name=operation,proto3,enum=dfuse.eosio.codec.v1.DTrxOp_Operation" json:"operation,omitempty"`
    public uint ActionIndex;//          uint32             `protobuf:"varint,2,opt,name=action_index,json=actionIndex,proto3" json:"action_index,omitempty"`
    public string Sender;//               string             `protobuf:"bytes,3,opt,name=sender,proto3" json:"sender,omitempty"`
    public string SenderId;//             string             `protobuf:"bytes,4,opt,name=sender_id,json=senderId,proto3" json:"sender_id,omitempty"`
    public string Payer;//                string             `protobuf:"bytes,5,opt,name=payer,proto3" json:"payer,omitempty"`
    public string PublishedAt;//          string             `protobuf:"bytes,6,opt,name=published_at,json=publishedAt,proto3" json:"published_at,omitempty"`
    public string DelayUntil;//           string             `protobuf:"bytes,7,opt,name=delay_until,json=delayUntil,proto3" json:"delay_until,omitempty"`
    public string ExpirationAt;//         string             `protobuf:"bytes,8,opt,name=expiration_at,json=expirationAt,proto3" json:"expiration_at,omitempty"`
    public string TransactionId;//        string             `protobuf:"bytes,9,opt,name=transaction_id,json=transactionId,proto3" json:"transaction_id,omitempty"`
    public SignedTransaction Transaction;//          *SignedTransaction `protobuf:"bytes,10,opt,name=transaction,proto3" json:"transaction,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}           `json:"-"`
    //XXX_unrecognized     []byte             `json:"-"`
    //XXX_sizecache        int32              `json:"-"`
}