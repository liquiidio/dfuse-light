namespace DeepReader;

public class RlimitOp
{
    public RlimitOp_Operation Operation;// RlimitOp_Operation; `protobuf:"varint,1,opt,name=operation,proto3,enum=dfuse.eosio.codec.v1.RlimitOp_Operation" json:"operation,omitempty"`
    // Types that are valid to be assigned to Kind:
    //	*RlimitOp_State
    //	*RlimitOp_Config
    //	*RlimitOp_AccountLimits
    //	*RlimitOp_AccountUsage
    public isRlimitOp_Kind Kind;//                 isRlimitOp_Kind `protobuf_oneof:"kind"`
    //XXX_NoUnkeyedLiteral struct{}        `json:"-"`
    //XXX_unrecognized     []byte          `json:"-"`
    //XXX_sizecache        int32           `json:"-"`
}