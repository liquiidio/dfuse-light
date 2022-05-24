using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types;

public class TableOp
{
    public TableOpOperation Operation { get; set; } = TableOpOperation.UNKNOWN;//TableOp_Operation
                                                                               //    public Name Payer { get; set; } = string.Empty;//string
    public Name Code { get; set; } = string.Empty;//string
    public Name Scope { get; set; } = string.Empty;//string
    public Name TableName { get; set; } = string.Empty;//string

    public TableOp()
    {

    }

    public static TableOp ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new TableOp()
        {
            Operation = (TableOpOperation)reader.ReadByte(),
            //            Payer = reader.ReadName(),
            Code = reader.ReadName(),
            Scope = reader.ReadName(),
            TableName = reader.ReadName()
        };

        return obj;
    }

    internal void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        //        writer.Write(Payer.Binary);
        writer.WriteName(Code);
        writer.WriteName(Scope);
        writer.WriteName(TableName);
    }
}