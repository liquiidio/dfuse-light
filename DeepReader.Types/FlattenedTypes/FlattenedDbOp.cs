using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.FlattenedTypes;

public struct FlattenedDbOp
{
    public DbOpOperation Operation = DbOpOperation.UNKNOWN;//DBOp_Operation
    public Name Code = string.Empty;//string
    public Name Scope = string.Empty;//string
    public Name TableName = string.Empty;//string
    public byte[] PrimaryKey = Array.Empty<byte>();//string
    public Name OldPayer = string.Empty;//string
    public Name NewPayer = string.Empty;//string
    public byte[] OldData = Array.Empty<byte>();//[]byte
    public byte[] NewData = Array.Empty<byte>();//[]byte

    public static FlattenedDbOp ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }
}