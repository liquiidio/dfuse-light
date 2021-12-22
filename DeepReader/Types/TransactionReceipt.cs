namespace DeepReader.Types;

public class TransactionReceipt
{
    public string Id;//                   string             `protobuf:"bytes,4,opt,name=id,proto3" json:"id,omitempty"`
    public ulong Index;//                uint64             `protobuf:"varint,6,opt,name=index,proto3" json:"index,omitempty"`
    public TransactionStatus Status;//               TransactionStatus  `protobuf:"varint,1,opt,name=status,proto3,enum=dfuse.eosio.codec.v1.TransactionStatus" json:"status,omitempty"`
    public uint CpuUsageMicroSeconds;// uint32             `protobuf:"varint,2,opt,name=cpu_usage_micro_seconds,json=cpuUsageMicroSeconds,proto3" json:"cpu_usage_micro_seconds,omitempty"`
    public uint NetUsageWords;//        uint32             `protobuf:"varint,3,opt,name=net_usage_words,json=netUsageWords,proto3" json:"net_usage_words,omitempty"`
    public PackedTransaction PackedTransaction;//    *PackedTransaction `protobuf:"bytes,5,opt,name=packed_transaction,json=packedTransaction,proto3" json:"packed_transaction,omitempty"`
    //XXX_NoUnkeyedLiteral;// struct{}           `json:"-"`
    //XXX_unrecognized;//     []byte             `json:"-"`
    //XXX_sizecache;//        int32              `json:"-"`
}