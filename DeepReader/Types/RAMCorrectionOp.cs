namespace DeepReader.Types;

public class RAMCorrectionOp
{
    public string CorrectionId;//         string   `protobuf:"bytes,1,opt,name=correction_id,json=correctionId,proto3" json:"correction_id,omitempty"`
    public string UniqueKey;//            string   `protobuf:"bytes,2,opt,name=unique_key,json=uniqueKey,proto3" json:"unique_key,omitempty"`
    public string Payer;//                string   `protobuf:"bytes,3,opt,name=payer,proto3" json:"payer,omitempty"`
    public long Delta;//                int64    `protobuf:"varint,4,opt,name=delta,proto3" json:"delta,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}