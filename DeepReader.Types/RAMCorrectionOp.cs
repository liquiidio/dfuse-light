using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

public sealed class RamCorrectionOp
{
    public ulong CorrectionId = 0;//string
    public ulong UniqueKey = 0;//string
    public Name Payer = Name.TypeEmpty;//string
    public long Delta = 0;//int64
}