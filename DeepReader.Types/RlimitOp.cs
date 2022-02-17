using DeepReader.Types.Enums;

namespace DeepReader.Types;


public abstract class RlimitOp
{
    public RlimitOpOperation Operation = RlimitOpOperation.UNKNOWN;//RlimitOp_Operation
    // Types that are valid to be assigned to Kind:
    //	*RlimitOp_State
    //	*RlimitOp_Config
    //	*RlimitOp_AccountLimits
    //	*RlimitOp_AccountUsage
    //  public isRlimitOp_Kind Kind;//isRlimitOp_Kind
}