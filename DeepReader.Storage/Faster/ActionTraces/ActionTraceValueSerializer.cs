using DeepReader.Types.StorageTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.ActionTraces;

public sealed class ActionTraceValueSerializer : BinaryObjectSerializer<ActionTrace>
{
    public override void Deserialize(out ActionTrace obj)
    {
        obj = ActionTrace.ReadFromBinaryReader(reader);
    }

    public override void Serialize(ref ActionTrace obj)
    {
        obj.WriteToBinaryWriter(writer);
    }
}