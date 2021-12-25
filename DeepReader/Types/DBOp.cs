namespace DeepReader.Types;

public class DBOp
{
    public DBOpOperation Operation;//            DBOp_Operation `protobuf:"varint,1,opt,name=operation,proto3,enum=dfuse.eosio.codec.v1.DBOp_Operation" json:"operation,omitempty"`
    public uint ActionIndex;//          uint32         `protobuf:"varint,2,opt,name=action_index,json=actionIndex,proto3" json:"action_index,omitempty"`
    public string Code;//                 string         `protobuf:"bytes,3,opt,name=code,proto3" json:"code,omitempty"`
    public string Scope;//                string         `protobuf:"bytes,4,opt,name=scope,proto3" json:"scope,omitempty"`
    public string TableName;//            string         `protobuf:"bytes,5,opt,name=table_name,json=tableName,proto3" json:"table_name,omitempty"`
    public string PrimaryKey;//           string         `protobuf:"bytes,6,opt,name=primary_key,json=primaryKey,proto3" json:"primary_key,omitempty"`
    public string OldPayer;//             string         `protobuf:"bytes,7,opt,name=old_payer,json=oldPayer,proto3" json:"old_payer,omitempty"`
    public string NewPayer;//             string         `protobuf:"bytes,8,opt,name=new_payer,json=newPayer,proto3" json:"new_payer,omitempty"`
    public byte[] OldData;//              []byte         `protobuf:"bytes,9,opt,name=old_data,json=oldData,proto3" json:"old_data,omitempty"`
    public byte[] NewData;//              []byte         `protobuf:"bytes,10,opt,name=new_data,json=newData,proto3" json:"new_data,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}       `json:"-"`
    //XXX_unrecognized     []byte         `json:"-"`
    //XXX_sizecache        int32          `json:"-"`
}