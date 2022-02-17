using DeepReader.Types.Enums;

namespace DeepReader.Types;

public class RamOp
{
    public RamOpOperation Operation = RamOpOperation.UNKNOWN;//RAMOp_Operation
    public uint ActionIndex = 0;// uint32
    public string Payer = string.Empty;//string
    public long Delta = 0;//int64
    public ulong Usage = 0;//uint64
    // To eventually replace `operation`.
    public RamOpNamespace Namespace = RamOpNamespace.UNKNOWN;//RAMOp_Namespace
    public string UniqueKey = string.Empty;//string
    public RamOpAction Action = RamOpAction.UNKNOWN;//RAMOp_Action
}