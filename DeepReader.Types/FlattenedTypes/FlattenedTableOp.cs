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

    public FlattenedTableOp()
    {

    }

    public static FlattenedTableOp ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new FlattenedTableOp()
        {
            Operation = (TableOpOperation) reader.ReadByte(),
            Payer = reader.ReadUInt64(),
            Code = reader.ReadUInt64(),
            Scope = reader.ReadUInt64(),
            TableName = reader.ReadUInt64()
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