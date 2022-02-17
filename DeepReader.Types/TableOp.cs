using DeepReader.Types.Enums;

namespace DeepReader.Types;

public class TableOp
{
    public TableOpOperation Operation = TableOpOperation.UNKNOWN;//TableOp_Operation
    public uint ActionIndex = 0;//uint32
    public string Payer = string.Empty;//string
    public string Code = string.Empty;//string
    public string Scope = string.Empty;//string
    public string TableName = string.Empty;//string

    public static TableOp ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}