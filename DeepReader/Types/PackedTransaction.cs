namespace DeepReader.Types;

public class PackedTransaction
{
    public string[] Signatures;//            []string `protobuf:"bytes,1,rep,name=signatures,proto3" json:"signatures,omitempty"`
    public uint Compression;//           uint32   `protobuf:"varint,2,opt,name=compression,proto3" json:"compression,omitempty"`
    public byte[] PackedContextFreeData;// []byte   `protobuf:"bytes,3,opt,name=packed_context_free_data,json=packedContextFreeData,proto3" json:"packed_context_free_data,omitempty"`
    public byte[] _PackedTransaction;//     []byte   `protobuf:"bytes,4,opt,name=packed_transaction,json=packedTransaction,proto3" json:"packed_transaction,omitempty"`
    //XXX_NoUnkeyedLiteral  struct{} `json:"-"`
    //XXX_unrecognized      []byte   `json:"-"`
    //XXX_sizecache         int32    `json:"-"`
}