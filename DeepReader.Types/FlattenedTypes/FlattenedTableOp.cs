using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.FlattenedTypes;

public struct FlattenedTableOp
{
    public TableOpOperation Operation = TableOpOperation.UNKNOWN;//TableOp_Operation
    public Name Payer = string.Empty;//string
    public Name Code = string.Empty;//string
    public Name Scope = string.Empty;//string
    public Name TableName = string.Empty;//string

    public static FlattenedTableOp ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }
}