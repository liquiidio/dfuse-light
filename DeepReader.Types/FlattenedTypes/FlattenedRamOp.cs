using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.FlattenedTypes;

public struct FlattenedRamOp
{
    public RamOpOperation Operation = RamOpOperation.UNKNOWN;//RAMOp_Operation
    public Name Payer = string.Empty;//string
    public long Delta = 0;//int64
    public ulong Usage = 0;//uint64
    // To eventually replace `operation`.
    public RamOpNamespace Namespace = RamOpNamespace.UNKNOWN;//RAMOp_Namespace
    public RamOpAction Action = RamOpAction.UNKNOWN;//RAMOp_Action

    public static FlattenedRamOp ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }
}