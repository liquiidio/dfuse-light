using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

public class RamCorrectionOp
{
    public ulong CorrectionId = 0;//string
    public ulong UniqueKey = 0;//string
    public Name Payer = Name.Empty;//string
    public long Delta = 0;//int64
}