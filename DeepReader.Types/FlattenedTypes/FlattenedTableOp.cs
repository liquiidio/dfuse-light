using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.FlattenedTypes;

public class FlattenedTableOp
{
    public TableOpOperation Operation { get; set; } = TableOpOperation.UNKNOWN;//TableOp_Operation
    public Name Payer { get; set; } = string.Empty;//string
    public Name Code { get; set; } = string.Empty;//string
    public Name Scope { get; set; } = string.Empty;//string
    public Name TableName { get; set; } = string.Empty;//string

    public FlattenedTableOp()
    {

    }

    public static FlattenedTableOp ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new FlattenedTableOp()
        {
            Operation = (TableOpOperation) reader.ReadByte(),
            Payer = reader.ReadName(),
            Code = reader.ReadName(),
            Scope = reader.ReadName(),
            TableName = reader.ReadName()
        };

        return obj;
    }

    internal void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        writer.Write(Payer.Binary);
        writer.Write(Code.Binary);
        writer.Write(Scope.Binary);
        writer.Write(TableName.Binary);
    }
}