namespace DeepReader.Types;

public class RlimitAccountUsage : RlimitOp
{
    public string Owner;//                string            `protobuf:"bytes,1,opt,name=owner,proto3" json:"owner,omitempty"`
    public UsageAccumulator NetUsage;//             *UsageAccumulator `protobuf:"bytes,2,opt,name=net_usage,json=netUsage,proto3" json:"net_usage,omitempty"`
    public UsageAccumulator CpuUsage;//             *UsageAccumulator `protobuf:"bytes,3,opt,name=cpu_usage,json=cpuUsage,proto3" json:"cpu_usage,omitempty"`
    public ulong RamUsage;//             uint64            `protobuf:"varint,4,opt,name=ram_usage,json=ramUsage,proto3" json:"ram_usage,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}          `json:"-"`
    //XXX_unrecognized     []byte            `json:"-"`
    //XXX_sizecache        int32             `json:"-"`
}