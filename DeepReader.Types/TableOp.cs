using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;

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

    public static TableOp ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        var obj = new TableOp()
        {
            Operation = (TableOpOperation)reader.ReadByte(),
            //            Payer = Name.ReadFromBinaryReader(reader),
            Code = Name.ReadFromBinaryReader(reader),
            Scope = Name.ReadFromBinaryReader(reader),
            TableName = Name.ReadFromBinaryReader(reader)
        };

        return obj;
    }

    internal void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        //        writer.Write(Payer.Binary);
        Code.WriteToBinaryWriter(writer);
        Scope.WriteToBinaryWriter(writer);
        TableName.WriteToBinaryWriter(writer);
    }
}