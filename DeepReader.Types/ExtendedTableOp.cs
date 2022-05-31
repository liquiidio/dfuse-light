using DeepReader.Types.EosTypes;

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
    //    Payer = Name.ReadFromBinaryReader(reader);
    //    Code = Name.ReadFromBinaryReader(reader);
    //    Scope = Name.ReadFromBinaryReader(reader);
    //    TableName = Name.ReadFromBinaryReader(reader);
    //}

    //public static ExtendedTableOp ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
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