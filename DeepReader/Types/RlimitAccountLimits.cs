namespace DeepReader.Types;

public class RlimitAccountLimits : RlimitOp
{
    public string Owner;//                string   `protobuf:"bytes,1,opt,name=owner,proto3" json:"owner,omitempty"`
    public bool Pending;//              bool     `protobuf:"varint,2,opt,name=pending,proto3" json:"pending,omitempty"`
    public long NetWeight;//            int64    `protobuf:"varint,3,opt,name=net_weight,json=netWeight,proto3" json:"net_weight,omitempty"`
    public long CpuWeight;//            int64    `protobuf:"varint,4,opt,name=cpu_weight,json=cpuWeight,proto3" json:"cpu_weight,omitempty"`
    public long RamBytes;//             int64    `protobuf:"varint,5,opt,name=ram_bytes,json=ramBytes,proto3" json:"ram_bytes,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}