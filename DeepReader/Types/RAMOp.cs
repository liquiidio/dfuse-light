using DeepReader.Types.Enums;

namespace DeepReader.Types;

public class RAMOp
{
    public RAMOp_Operation Operation = RAMOp_Operation.UNKNOWN;//RAMOp_Operation
    public uint ActionIndex = 0;// uint32
    public string Payer = string.Empty;//string
    public long Delta = 0;//int64
    public ulong Usage = 0;//uint64
    // To eventually replace `operation`.
    public RAMOp_Namespace Namespace = RAMOp_Namespace.UNKNOWN;//RAMOp_Namespace
    public string UniqueKey = string.Empty;//string
    public RAMOpAction Action = RAMOpAction.UNKNOWN;//RAMOp_Action
}