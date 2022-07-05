using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;

namespace DeepReader.Types;

public class TableOp : IEosioSerializable<TableOp>, IFasterSerializable<TableOp>
{
    public TableOpOperation Operation { get; set; } = TableOpOperation.UNKNOWN;//TableOp_Operation
                                                                               //    public Name Payer { get; set; } = string.Empty;//string
    public Name Code { get; set; } = string.Empty;//string
    public Name Scope { get; set; } = string.Empty;//string
    public Name TableName { get; set; } = string.Empty;//string

    public TableOp()
    {

    }

    public static TableOp ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
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

    public static TableOp ReadFromFaster(IBufferReader reader, bool fromPool = true)
    {
        var obj = new TableOp()
        {
            Operation = (TableOpOperation)reader.ReadByte(),
            //            Payer = Name.ReadFromBinaryReader(reader),
            Code = Name.ReadFromFaster(reader),
            Scope = Name.ReadFromFaster(reader),
            TableName = Name.ReadFromFaster(reader)
        };

        return obj;
    }

    public void WriteToFaster(IBufferWriter writer)
    {
        writer.Write((byte)Operation);
        //        writer.Write(Payer.Binary);
        Code.WriteToFaster(writer);
        Scope.WriteToFaster(writer);
        TableName.WriteToFaster(writer);
    }
}