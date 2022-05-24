using FASTER.core;

namespace DeepReader.Storage.Faster.ActionTraces;

public class ActionTraceIdSerializer : BinaryObjectSerializer<ActionTraceId>
{
    public override void Deserialize(out ActionTraceId obj)
    {
        obj = new ActionTraceId(reader.ReadUInt64());
    }

    public override void Serialize(ref ActionTraceId obj)
    {
        writer.Write(obj.GlobalSequence);
    }
}