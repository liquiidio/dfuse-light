using DeepReader.Types.Enums;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.FlattenedTypes;

public struct FlattenedRamOp
{
    public RamOpOperation Operation = RamOpOperation.UNKNOWN;//RAMOp_Operation
    public Name Payer = string.Empty;//string
    public long Delta = 0;//int64
    public ulong Usage = 0;//uint64
    // To eventually replace `operation`.
    public RamOpNamespace Namespace = RamOpNamespace.UNKNOWN;//RAMOp_Namespace
    public RamOpAction Action = RamOpAction.UNKNOWN;//RAMOp_Action

    public FlattenedRamOp()
    {

    }

    public static FlattenedRamOp ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new FlattenedRamOp()
        {
            Operation = (RamOpOperation) reader.ReadByte(),
            Payer = reader.ReadUInt64(),
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