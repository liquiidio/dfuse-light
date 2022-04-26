using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.FlattenedTypes;

public class FlattenedDbOp
{
    public DbOpOperation Operation { get; set; } = DbOpOperation.UNKNOWN;//DBOp_Operation
    public Name Code { get; set; } = string.Empty;//string
    public Name Scope { get; set; } = string.Empty;//string
    public Name TableName { get; set; } = string.Empty;//string
    public byte[] PrimaryKey { get; set; } = Array.Empty<byte>();//string
    public Name OldPayer { get; set; } = string.Empty;//string
    public Name NewPayer { get; set; } = string.Empty;//string
    public ReadOnlyMemory<byte> OldData { get; set; } = Array.Empty<byte>();//[]byte
    public ReadOnlyMemory<byte> NewData { get; set; } = Array.Empty<byte>();//[]byte

    public FlattenedDbOp()
    {

    }

    public static FlattenedDbOp ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new FlattenedDbOp()
        {
            Operation = (DbOpOperation) reader.ReadByte(),
            Code = reader.ReadUInt64(),
            Scope = reader.ReadUInt64(),
            TableName = reader.ReadUInt64(),
            PrimaryKey = reader.ReadBytes(reader.ReadInt32()),
            OldPayer = reader.ReadUInt64(),
            NewPayer = reader.ReadUInt64(),
            OldData = reader.ReadBytes(reader.ReadInt32()),
            NewData = reader.ReadBytes(reader.ReadInt32())
        };

        return obj;
    }

    internal void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        writer.Write(Code.Binary);
        writer.Write(Scope.Binary);
        writer.Write(TableName.Binary);
        writer.Write(PrimaryKey.Length);
        writer.Write(PrimaryKey);
        writer.Write(OldPayer.Binary);
        writer.Write(NewPayer.Binary);
        writer.Write(OldData.Length);
        writer.Write(OldData.Span);
        writer.Write(NewData.Length);
        writer.Write(NewData.Span);
    }
}