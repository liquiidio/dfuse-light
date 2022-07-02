using DeepReader.Types.StorageTypes;
using FASTER.core;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Infrastructure.BinaryReaders;

namespace DeepReader.Storage.Faster.ActionTraces.Standalone;

public sealed class ActionTraceValueSerializer : BinaryObjectSerializer<ActionTrace>
{
    public override void Deserialize(out ActionTrace obj)
    {
        // TODO
        obj = ActionTrace.ReadFromFaster((IBufferReader)reader);
    }

    public override void Serialize(ref ActionTrace obj)
    {
        // TODO
        obj.WriteToFaster((IBufferWriter)writer);
    }
}