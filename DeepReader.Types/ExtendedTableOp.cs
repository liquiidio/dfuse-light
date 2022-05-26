using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types;

public sealed class ExtendedTableOp : TableOp
{
    public uint ActionIndex { get; set; }//uint32
    public Name Payer { get; set; } = Name.TypeEmpty;//string

    public ExtendedTableOp() : base()
    {

    }

    //public ExtendedTableOp(BinaryReader reader)
    //{
    //    Operation = (TableOpOperation) reader.ReadByte();
    //    ActionIndex = reader.ReadUInt32();
    //    Payer = reader.ReadName();
    //    Code = reader.ReadName();
    //    Scope = reader.ReadName();
    //    TableName = reader.ReadName();
    //}

    //public static ExtendedTableOp ReadFromBinaryReader(BinaryReader reader)
    //{
    //    return new ExtendedTableOp(reader);
    //}

    //public void WriteToBinaryWriter(BinaryWriter writer)
    //{
    //    writer.Write((byte)Operation);
    //    writer.Write(ActionIndex);
    //    writer.WriteName(Payer);
    //    writer.WriteName(Code);
    //    writer.WriteName(Scope);
    //    writer.WriteName(TableName);
    //}
}