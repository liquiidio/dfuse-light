using System.Text.Json.Serialization;
using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types;

public class DbOp
{
    public DbOpOperation Operation = DbOpOperation.UNKNOWN;//DBOp_Operation
    public uint ActionIndex = 0;//uint32
    public Name Code = string.Empty;//string
    public Name Scope = string.Empty;//string
    public Name TableName = string.Empty;//string
    public string PrimaryKey = string.Empty;//string
    public Name OldPayer = string.Empty;//string
    public Name NewPayer = string.Empty;//string
    [JsonIgnore]
    public ReadOnlyMemory<byte> OldData = Array.Empty<byte>();//[]byte
    [JsonIgnore]
    public ReadOnlyMemory<byte> NewData = Array.Empty<byte>();//[]byte

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