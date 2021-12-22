namespace DeepReader.Types;

public class RlimitState : RlimitOp
{
    public UsageAccumulator AverageBlockNetUsage;// *UsageAccumulator `protobuf:"bytes,1,opt,name=average_block_net_usage,json=averageBlockNetUsage,proto3" json:"average_block_net_usage,omitempty"`
    public UsageAccumulator AverageBlockCpuUsage;// *UsageAccumulator `protobuf:"bytes,2,opt,name=average_block_cpu_usage,json=averageBlockCpuUsage,proto3" json:"average_block_cpu_usage,omitempty"`
    public ulong PendingNetUsage;//      uint64            `protobuf:"varint,3,opt,name=pending_net_usage,json=pendingNetUsage,proto3" json:"pending_net_usage,omitempty"`
    public ulong PendingCpuUsage;//      uint64            `protobuf:"varint,4,opt,name=pending_cpu_usage,json=pendingCpuUsage,proto3" json:"pending_cpu_usage,omitempty"`
    public ulong TotalNetWeight;//       uint64            `protobuf:"varint,5,opt,name=total_net_weight,json=totalNetWeight,proto3" json:"total_net_weight,omitempty"`
    public ulong TotalCpuWeight;//       uint64            `protobuf:"varint,6,opt,name=total_cpu_weight,json=totalCpuWeight,proto3" json:"total_cpu_weight,omitempty"`
    public ulong TotalRamBytes;//        uint64            `protobuf:"varint,7,opt,name=total_ram_bytes,json=totalRamBytes,proto3" json:"total_ram_bytes,omitempty"`
    public ulong VirtualNetLimit;//      uint64            `protobuf:"varint,8,opt,name=virtual_net_limit,json=virtualNetLimit,proto3" json:"virtual_net_limit,omitempty"`
    public ulong VirtualCpuLimit;//      uint64            `protobuf:"varint,9,opt,name=virtual_cpu_limit,json=virtualCpuLimit,proto3" json:"virtual_cpu_limit,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}          `json:"-"`
    //XXX_unrecognized     []byte            `json:"-"`
    //XXX_sizecache        int32             `json:"-"`
}