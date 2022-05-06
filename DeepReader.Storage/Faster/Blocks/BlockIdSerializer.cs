using FASTER.core;

namespace DeepReader.Storage.Faster.Blocks;

public class BlockIdSerializer : BinaryObjectSerializer<BlockId>
{
    public override void Deserialize(out BlockId obj)
    {
        obj = new BlockId(reader.ReadInt64());
    }

    public override void Serialize(ref BlockId obj)
    {
        writer.Write(obj.Id);
    }
}