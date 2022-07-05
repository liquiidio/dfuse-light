using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;

namespace DeepReader.Types;

public class RamOp : IEosioSerializable<RamOp>, IFasterSerializable<RamOp>
{
    public RamOpOperation Operation { get; set; } = RamOpOperation.UNKNOWN;//RAMOp_Operation
    public Name Payer { get; set; } = string.Empty;//string
    public long Delta { get; set; } = 0;//int64
    public ulong Usage { get; set; } = 0;//uint64

    public RamOp()
    {

    }

    public static RamOp ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
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

    public static RamOp ReadFromFaster(IBufferReader reader, bool fromPool = true)
    {
        var obj = new RamOp()
        {
            Operation = (RamOpOperation)reader.ReadByte(),
            Payer = Name.ReadFromFaster(reader),
            Delta = reader.ReadInt64(),
            Usage = reader.ReadUInt64()
        };

        return obj;
    }

    public void WriteToFaster(IBufferWriter writer)
    {
        writer.Write((byte)Operation);
        Payer.WriteToFaster(writer);
        writer.Write(Delta);
        writer.Write(Usage);
    }
}