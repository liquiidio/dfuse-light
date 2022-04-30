using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

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

    public FlattenedDbOp(BinaryReader reader)
    {
        Operation = (DbOpOperation)reader.ReadByte();
        Code = reader.ReadName();
        Scope = reader.ReadName();
        TableName = reader.ReadName();
        PrimaryKey = reader.ReadBytes(reader.ReadInt32());
        switch (Operation)
        {
            case DbOpOperation.UNKNOWN:
                break;
            case DbOpOperation.INS: // has only newpayer and newdata
                NewPayer = reader.ReadName();
                NewData = reader.ReadBytes(reader.Read7BitEncodedInt());
                break;
            case DbOpOperation.UPD: // has all
                NewPayer = reader.ReadName();
                NewData = reader.ReadBytes(reader.Read7BitEncodedInt());
                OldPayer = reader.ReadName();
                OldData = reader.ReadBytes(reader.Read7BitEncodedInt());
                break;
            case DbOpOperation.REM: // has only oldpayer and olddata
                OldPayer = reader.ReadName();
                OldData = reader.ReadBytes(reader.Read7BitEncodedInt());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static FlattenedDbOp ReadFromBinaryReader(BinaryReader reader)
    {
        return new FlattenedDbOp(reader);
    }

    internal void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        writer.WriteName(Code);
        writer.WriteName(Scope);
        writer.WriteName(TableName);
        writer.Write(PrimaryKey.Length);
        writer.Write(PrimaryKey);
        switch (Operation)
        {
            case DbOpOperation.UNKNOWN:
                break;
            case DbOpOperation.INS: // has only newpayer and newdata
                writer.WriteName(NewPayer);
                writer.Write7BitEncodedInt(NewData.Length);
                writer.Write(NewData.Span);
                break;
            case DbOpOperation.UPD: // has all
                writer.WriteName(NewPayer);
                writer.Write7BitEncodedInt(NewData.Length);
                writer.Write(NewData.Span);
                writer.WriteName(OldPayer);
                writer.Write7BitEncodedInt(OldData.Length);
                writer.Write(OldData.Span);
                break;
            case DbOpOperation.REM: // has only oldpayer and olddata
                writer.WriteName(OldPayer);
                writer.Write7BitEncodedInt(OldData.Length);
                writer.Write(OldData.Span);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}