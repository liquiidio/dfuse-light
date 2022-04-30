using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.FlattenedTypes;

public class FlattenedRamOp
{
    public RamOpOperation Operation { get; set; } = RamOpOperation.UNKNOWN;//RAMOp_Operation
    public Name Payer { get; set; } = string.Empty;//string
    public long Delta { get; set; } = 0;//int64
    public ulong Usage { get; set; } = 0;//uint64
    // To eventually replace `operation`.
    public RamOpNamespace Namespace { get; set; } = RamOpNamespace.UNKNOWN;//RAMOp_Namespace
    public RamOpAction Action { get; set; } = RamOpAction.UNKNOWN;//RAMOp_Action

    public FlattenedRamOp()
    {

    }

    public static FlattenedRamOp ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new FlattenedRamOp()
        {
            Operation = (RamOpOperation) reader.ReadByte(),
            Payer = reader.ReadName(),
            Delta = reader.ReadInt64(),
            Usage = reader.ReadUInt64(),
            Namespace = (RamOpNamespace) reader.ReadByte(),
            Action = (RamOpAction) reader.ReadByte()
        };

        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write((byte)Operation);
        writer.Write(Payer.Binary);
        writer.Write(Delta);
        writer.Write(Usage);
        writer.Write((byte)Namespace);
        writer.Write((byte)Action);
    }
}