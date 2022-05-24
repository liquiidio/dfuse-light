using System.Text.Json.Serialization;
using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types;

public class ExtendedDbOp : DbOp
{
    public uint ActionIndex { get; set; } = 0;//uint32
    public ExtendedDbOp() : base()
    {

    }

    //public ExtendedDbOp(BinaryReader reader)
    //{
    //    Operation = (DbOpOperation)reader.ReadByte();
    //    ActionIndex = reader.ReadUInt32();
    //    Code = reader.ReadName();
    //    Scope = reader.ReadName();
    //    TableName = reader.ReadName();
    //    PrimaryKey = reader.ReadString();
    //    switch (Operation)
    //    {
    //        case DbOpOperation.UNKNOWN:
    //            break;
    //        case DbOpOperation.INS: // has only newpayer and newdata
    //            NewPayer = reader.ReadName();
    //            NewData = reader.ReadBytes(reader.Read7BitEncodedInt());
    //            break;
    //        case DbOpOperation.UPD: // has all
    //            NewPayer = reader.ReadName();
    //            NewData = reader.ReadBytes(reader.Read7BitEncodedInt());
    //            OldPayer = reader.ReadName();
    //            OldData = reader.ReadBytes(reader.Read7BitEncodedInt());
    //            break;
    //        case DbOpOperation.REM: // has only oldpayer and olddata
    //            OldPayer = reader.ReadName();
    //            OldData = reader.ReadBytes(reader.Read7BitEncodedInt());
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }
    //}

    //internal static ExtendedDbOp ReadFromBinaryReader(BinaryReader reader)
    //{
    //    return new ExtendedDbOp(reader);
    //}

    //public void WriteToBinaryWriter(BinaryWriter writer)
    //{
    //    writer.Write((byte)Operation);
    //    writer.Write(ActionIndex);
    //    writer.WriteName(Code);
    //    writer.WriteName(Scope);
    //    writer.WriteName(TableName);
    //    writer.Write(PrimaryKey);
    //    switch (Operation)
    //    {
    //        case DbOpOperation.UNKNOWN:
    //            break;
    //        case DbOpOperation.INS: // has only newpayer and newdata
    //            writer.WriteName(NewPayer);
    //            writer.Write7BitEncodedInt(NewData.Length);
    //            writer.Write(NewData.Span);
    //            break;
    //        case DbOpOperation.UPD: // has all
    //            writer.WriteName(NewPayer);
    //            writer.Write7BitEncodedInt(NewData.Length);
    //            writer.Write(NewData.Span);
    //            writer.WriteName(OldPayer);
    //            writer.Write7BitEncodedInt(OldData.Length);
    //            writer.Write(OldData.Span);
    //            break;
    //        case DbOpOperation.REM: // has only oldpayer and olddata
    //            writer.WriteName(OldPayer);
    //            writer.Write7BitEncodedInt(OldData.Length);
    //            writer.Write(OldData.Span);
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException();
    //    }
    //}
}