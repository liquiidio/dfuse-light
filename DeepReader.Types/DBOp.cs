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

    internal static DbOp ReadFromBinaryReader(BinaryReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        writer.Write(ActionIndex);
        writer.WriteName(Code);
        writer.WriteName(Scope);
        writer.WriteName(TableName);
        writer.Write(PrimaryKey);
        writer.WriteName(OldPayer);
        writer.WriteName(NewPayer);
    }
}