namespace DeepReader.Types;

public class TableOp
{
    public TableOp_Operation Operation;//            TableOp_Operation `protobuf:"varint,1,opt,name=operation,proto3,enum=dfuse.eosio.codec.v1.TableOp_Operation" json:"operation,omitempty"`
    public uint ActionIndex;//          uint32            `protobuf:"varint,2,opt,name=action_index,json=actionIndex,proto3" json:"action_index,omitempty"`
    public string Payer;//                string            `protobuf:"bytes,3,opt,name=payer,proto3" json:"payer,omitempty"`
    public string Code;//                 string            `protobuf:"bytes,4,opt,name=code,proto3" json:"code,omitempty"`
    public string Scope;//                string            `protobuf:"bytes,5,opt,name=scope,proto3" json:"scope,omitempty"`
    public string TableName;//            string            `protobuf:"bytes,6,opt,name=table_name,json=tableName,proto3" json:"table_name,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}          `json:"-"`
    //XXX_unrecognized     []byte            `json:"-"`
    //XXX_sizecache        int32             `json:"-"`
}