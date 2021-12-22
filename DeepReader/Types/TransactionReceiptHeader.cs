namespace DeepReader.Types;

public class TransactionReceiptHeader
{
    public TransactionStatus Status;//               TransactionStatus `protobuf:"varint,1,opt,name=status,proto3,enum=dfuse.eosio.codec.v1.TransactionStatus" json:"status,omitempty"`
    public uint CpuUsageMicroSeconds;// uint32            `protobuf:"varint,2,opt,name=cpu_usage_micro_seconds,json=cpuUsageMicroSeconds,proto3" json:"cpu_usage_micro_seconds,omitempty"`
    public uint NetUsageWords;//        uint32            `protobuf:"varint,3,opt,name=net_usage_words,json=netUsageWords,proto3" json:"net_usage_words,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}          `json:"-"`
    //XXX_unrecognized     []byte            `json:"-"`
    //XXX_sizecache        int32             `json:"-"`
}