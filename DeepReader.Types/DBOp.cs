using System.Text.Json.Serialization;
using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types;

[JsonConverter(typeof(DbOpJsonConverter))]
public class DbOp
{
    public DbOpOperation Operation { get; set; } = DbOpOperation.UNKNOWN;//DBOp_Operation
    public Name Code { get; set; } = string.Empty;//string
    public Name TableName { get; set; } = string.Empty;//string
    public Name Scope { get; set; } = string.Empty;//string
    public ReadOnlyMemory<char> PrimaryKey { get; set; } = Array.Empty<char>();//string
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
        Code = Name.ReadFromBinaryReader(reader);
        Scope = Name.ReadFromBinaryReader(reader);
        TableName = Name.ReadFromBinaryReader(reader);
        var length = reader.Read7BitEncodedInt();
        PrimaryKey = reader.ReadChars(length);
        switch (Operation)
        {
            case DbOpOperation.UNKNOWN:
                break;
            case DbOpOperation.INS: // has only newpayer and newdata
                NewPayer = Name.ReadFromBinaryReader(reader);
                NewData = reader.ReadBytes(reader.Read7BitEncodedInt());
                break;
            case DbOpOperation.UPD: // has all
                NewPayer = Name.ReadFromBinaryReader(reader);
                NewData = reader.ReadBytes(reader.Read7BitEncodedInt());
                OldPayer = Name.ReadFromBinaryReader(reader);
                OldData = reader.ReadBytes(reader.Read7BitEncodedInt());
                break;
            case DbOpOperation.REM: // has only oldpayer and olddata
                OldPayer = Name.ReadFromBinaryReader(reader);
                OldData = reader.ReadBytes(reader.Read7BitEncodedInt());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static DbOp ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        return new DbOp(reader);
    }

    internal void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        Code.WriteToBinaryWriter(writer);
        Scope.WriteToBinaryWriter(writer);
        TableName.WriteToBinaryWriter(writer);
        writer.Write7BitEncodedInt(PrimaryKey.Span.Length);
        writer.Write(PrimaryKey.Span);
        switch (Operation)
        {
            case DbOpOperation.UNKNOWN:
                break;
            case DbOpOperation.INS: // has only newpayer and newdata
                NewPayer.WriteToBinaryWriter(writer);
                writer.Write7BitEncodedInt(NewData.Length);
                writer.Write(NewData.Span);
                break;
            case DbOpOperation.UPD: // has all
                NewPayer.WriteToBinaryWriter(writer);
                writer.Write7BitEncodedInt(NewData.Length);
                writer.Write(NewData.Span);
                OldPayer.WriteToBinaryWriter(writer);
                writer.Write7BitEncodedInt(OldData.Length);
                writer.Write(OldData.Span);
                break;
            case DbOpOperation.REM: // has only oldpayer and olddata
                OldPayer.WriteToBinaryWriter(writer);
                writer.Write7BitEncodedInt(OldData.Length);
                writer.Write(OldData.Span);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}