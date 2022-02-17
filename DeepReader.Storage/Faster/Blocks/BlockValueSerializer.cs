using DeepReader.Types.FlattenedTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.Blocks;

public class BlockValueSerializer : BinaryObjectSerializer<FlattenedBlock>
{
    public override void Deserialize(out FlattenedBlock obj)
    {
        obj = FlattenedBlock.ReadFromBinaryReader(reader);
    }

    public override void Serialize(ref FlattenedBlock obj)
    {
        obj.WriteToBinaryWriter(writer);
    }
}