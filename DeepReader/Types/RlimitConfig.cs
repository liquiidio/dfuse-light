namespace DeepReader.Types;

public class RlimitConfig : RlimitOp
{
    public ElasticLimitParameters CpuLimitParameters;//           *ElasticLimitParameters `protobuf:"bytes,1,opt,name=cpu_limit_parameters,json=cpuLimitParameters,proto3" json:"cpu_limit_parameters,omitempty"`
    public ElasticLimitParameters NetLimitParameters;//           *ElasticLimitParameters `protobuf:"bytes,2,opt,name=net_limit_parameters,json=netLimitParameters,proto3" json:"net_limit_parameters,omitempty"`
    public uint AccountCpuUsageAverageWindow;// uint32                  `protobuf:"varint,3,opt,name=account_cpu_usage_average_window,json=accountCpuUsageAverageWindow,proto3" json:"account_cpu_usage_average_window,omitempty"`
    public uint AccountNetUsageAverageWindow;// uint32                  `protobuf:"varint,4,opt,name=account_net_usage_average_window,json=accountNetUsageAverageWindow,proto3" json:"account_net_usage_average_window,omitempty"`
    //XXX_NoUnkeyedLiteral         struct{}                `json:"-"`
    //XXX_unrecognized             []byte                  `json:"-"`
    //XXX_sizecache                int32                   `json:"-"`
}