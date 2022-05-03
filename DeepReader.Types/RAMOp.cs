using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using KGySoft.CoreLibraries;

namespace DeepReader.Types;

public class RamOp
{
    public RamOpOperation Operation = RamOpOperation.UNKNOWN;//RAMOp_Operation
    public uint ActionIndex = 0;// uint32
    public Name Payer = Name.Empty;//string
    public long Delta = 0;//int64
    public ulong Usage = 0;//uint64
    // To eventually replace `operation`.
    public RamOpNamespace Namespace = RamOpNamespace.UNKNOWN;//RAMOp_Namespace
    public IList<StringSegment> UniqueKey = new List<StringSegment>();//string
    public RamOpAction Action = RamOpAction.UNKNOWN;//RAMOp_Action
}