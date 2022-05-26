using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using KGySoft.CoreLibraries;

namespace DeepReader.Types;

public sealed class ExtendedRamOp : RamOp
{
    public uint ActionIndex = 0;// uint32

    // To eventually replace `operation`.
    public RamOpNamespace Namespace = RamOpNamespace.UNKNOWN;//RAMOp_Namespace
    public IList<StringSegment> UniqueKey = new List<StringSegment>();//string
    public RamOpAction Action = RamOpAction.UNKNOWN;//RAMOp_Action
}