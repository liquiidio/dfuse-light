using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types;

public class RamOp
{
    public RamOpOperation Operation { get; set; } = RamOpOperation.UNKNOWN;//RAMOp_Operation
    public Name Payer { get; set; } = string.Empty;//string
    public long Delta { get; set; } = 0;//int64
    public ulong Usage { get; set; } = 0;//uint64

    public RamOp()
    {

    }

    public static RamOp ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new RamOp()
        {
            Operation = (RamOpOperation)reader.ReadByte(),
            Payer = reader.ReadName(),
            Delta = reader.ReadInt64(),
            Usage = reader.ReadUInt64()
        };

        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        writer.Write(Payer.Binary);
        writer.Write(Delta);
        writer.Write(Usage);
    }
}