using DeepReader.Types.Enums;

namespace DeepReader.Types;

public class DbOp
{
    public DbOpOperation Operation = DbOpOperation.UNKNOWN;//DBOp_Operation
    public uint ActionIndex = 0;//uint32
    public string Code = string.Empty;//string
    public string Scope = string.Empty;//string
    public string TableName = string.Empty;//string
    public string PrimaryKey = string.Empty;//string
    public string OldPayer = string.Empty;//string
    public string NewPayer = string.Empty;//string
    public byte[] OldData = Array.Empty<byte>();//[]byte
    public byte[] NewData = Array.Empty<byte>();//[]byte

    internal static DbOp ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}