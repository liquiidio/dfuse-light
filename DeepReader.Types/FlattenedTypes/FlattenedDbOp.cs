﻿using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.FlattenedTypes;

public struct FlattenedDbOp
{
    public DbOpOperation Operation = DbOpOperation.UNKNOWN;//DBOp_Operation
    public Name Code = Name.Empty;//string
    public Name Scope = Name.Empty;//string
    public Name TableName = Name.Empty;//string
    public byte[] PrimaryKey = Array.Empty<byte>();//string
    public Name OldPayer = Name.Empty;//string
    public Name NewPayer = Name.Empty;//string
    public ReadOnlyMemory<byte> OldData = Array.Empty<byte>();//[]byte
    public ReadOnlyMemory<byte> NewData = Array.Empty<byte>();//[]byte

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