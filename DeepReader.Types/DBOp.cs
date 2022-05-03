using System.Text.Json.Serialization;
using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types;

public class DbOp
{
    public DbOpOperation Operation { get; set; } = DbOpOperation.UNKNOWN;//DBOp_Operation
    public uint ActionIndex { get; set; } = 0;//uint32
    public Name Code { get; set; } = string.Empty;//string
    public Name Scope { get; set; } = string.Empty;//string
    public Name TableName { get; set; } = string.Empty;//string
    public string PrimaryKey { get; set; } = string.Empty;//string
    public Name OldPayer { get; set; } = string.Empty;//string
    public Name NewPayer { get; set; } = string.Empty;//string
    [JsonIgnore]
    public ReadOnlyMemory<byte> OldData { get; set; } = Array.Empty<byte>();//[]byte
    [JsonIgnore]
    public ReadOnlyMemory<byte> NewData { get; set; } = Array.Empty<byte>();//[]byte

    public DbOp()
    {

    }

    public DbOp(BinaryReader reader)
    {
        Operation = (DbOpOperation)reader.ReadByte();
        ActionIndex = reader.ReadUInt32();
        Code = reader.ReadName();
        Scope = reader.ReadName();
        TableName = reader.ReadName();
        PrimaryKey = reader.ReadString();
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

    internal static DbOp ReadFromBinaryReader(BinaryReader reader)
    {
        return new DbOp(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        writer.Write(ActionIndex);
        writer.WriteName(Code);
        writer.WriteName(Scope);
        writer.WriteName(TableName);
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