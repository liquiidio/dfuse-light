namespace DeepReader;

public class ProducerToLastImpliedIRB
{
    public string Name;//                 string   `protobuf:"bytes,1,opt,name=name,proto3" json:"name,omitempty"`
    public uint LastBlockNumProduced;// uint32   `protobuf:"varint,2,opt,name=last_block_num_produced,json=lastBlockNumProduced,proto3" json:"last_block_num_produced,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}