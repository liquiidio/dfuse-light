using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types;

public class TableOp
{
    public TableOpOperation Operation = TableOpOperation.UNKNOWN;//TableOp_Operation
    public uint ActionIndex = 0;//uint32
    public Name Payer = string.Empty;//string
    public Name Code = string.Empty;//string
    public Name Scope = string.Empty;//string
    public Name TableName = string.Empty;//string

    public static TableOp ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        writer.Write(ActionIndex);
        writer.WriteName(Payer);
        writer.WriteName(Code);
        writer.WriteName(Scope);
        writer.WriteName(TableName);
    }
}