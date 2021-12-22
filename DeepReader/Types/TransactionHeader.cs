namespace DeepReader.Types;

public class TransactionHeader
{
    public Timestamp Expiration;//           *timestamp.Timestamp `protobuf:"bytes,1,opt,name=expiration,proto3" json:"expiration,omitempty"`
    public uint RefBlockNum;//          uint32               `protobuf:"varint,2,opt,name=ref_block_num,json=refBlockNum,proto3" json:"ref_block_num,omitempty"`
    public uint RefBlockPrefix;//       uint32               `protobuf:"varint,3,opt,name=ref_block_prefix,json=refBlockPrefix,proto3" json:"ref_block_prefix,omitempty"`
    public uint MaxNetUsageWords;//     uint32               `protobuf:"varint,4,opt,name=max_net_usage_words,json=maxNetUsageWords,proto3" json:"max_net_usage_words,omitempty"`
    public uint MaxCpuUsageMs;//        uint32               `protobuf:"varint,5,opt,name=max_cpu_usage_ms,json=maxCpuUsageMs,proto3" json:"max_cpu_usage_ms,omitempty"`
    public uint DelaySec;//             uint32               `protobuf:"varint,6,opt,name=delay_sec,json=delaySec,proto3" json:"delay_sec,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}             `json:"-"`
    //XXX_unrecognized     []byte               `json:"-"`
    //sXXX_sizecache        int32                `json:"-"`
}