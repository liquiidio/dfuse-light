using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.FlattenedTypes;

public struct FlattenedDbOp
{
    public DbOpOperation Operation = DbOpOperation.UNKNOWN;//DBOp_Operation
    public Name Code = string.Empty;//string
    public Name Scope = string.Empty;//string
    public Name TableName = string.Empty;//string
    public byte[] PrimaryKey = Array.Empty<byte>();//string
    public Name OldPayer = string.Empty;//string
    public Name NewPayer = string.Empty;//string
    public byte[] OldData = Array.Empty<byte>();//[]byte
    public byte[] NewData = Array.Empty<byte>();//[]byte

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
        writer.Write(OldData);
        writer.Write(NewData.Length);
        writer.Write(NewData);
    }
}