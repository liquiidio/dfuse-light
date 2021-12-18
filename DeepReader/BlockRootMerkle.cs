namespace DeepReader;

public class BlockRootMerkle
{
    public uint NodeCount;//            uint32   `protobuf:"varint,1,opt,name=node_count,json=nodeCount,proto3" json:"node_count,omitempty"`
    public byte[][] ActiveNodes;//          [][]byte `protobuf:"bytes,2,rep,name=active_nodes,json=activeNodes,proto3" json:"active_nodes,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}