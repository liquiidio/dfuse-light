using DeepReader.Types.StorageTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.Blocks.Standalone;

public sealed class BlockValueSerializer : BinaryObjectSerializer<Block>
{
    public override void Deserialize(out Block obj)
    {
        obj = Block.ReadFromFaster(reader);
    }

    public override void Serialize(ref Block obj)
    {
        obj.WriteToFaster(writer);
    }
}