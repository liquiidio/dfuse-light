namespace DeepReader.Types;

public class ElasticLimitParameters
{
    public ulong Target;//               uint64   `protobuf:"varint,1,opt,name=target,proto3" json:"target,omitempty"`
    public ulong Max;//                  uint64   `protobuf:"varint,2,opt,name=max,proto3" json:"max,omitempty"`
    public uint Periods;//              uint32   `protobuf:"varint,3,opt,name=periods,proto3" json:"periods,omitempty"`
    public uint MaxMultiplier;//        uint32   `protobuf:"varint,4,opt,name=max_multiplier,json=maxMultiplier,proto3" json:"max_multiplier,omitempty"`
    public Ratio ContractRate;//         *Ratio   `protobuf:"bytes,5,opt,name=contract_rate,json=contractRate,proto3" json:"contract_rate,omitempty"`
    public Ratio ExpandRate;//           *Ratio   `protobuf:"bytes,6,opt,name=expand_rate,json=expandRate,proto3" json:"expand_rate,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}