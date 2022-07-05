using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

public class RamOp
{
    public ulong Id { get; set; }

    public RamOpOperation Operation { get; set; } = RamOpOperation.UNKNOWN;//RAMOp_Operation
    public Name Payer { get; set; } = string.Empty;//string
    public long Delta { get; set; } = 0;//int64
    public ulong Usage { get; set; } = 0;//uint64

    public RamOp()
    {

    }

    public static RamOp ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        var obj = new RamOp()
        {
            Operation = (RamOpOperation)reader.ReadByte(),
            Payer = Name.ReadFromBinaryReader(reader),
            Delta = reader.ReadInt64(),
            Usage = reader.ReadUInt64()
        };

        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        Payer.WriteToBinaryWriter(writer);
        writer.Write(Delta);
        writer.Write(Usage);
    }
}