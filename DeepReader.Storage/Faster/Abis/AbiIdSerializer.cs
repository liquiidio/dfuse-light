using DeepReader.Storage.Faster.Abis;
using FASTER.core;

namespace DeepReader.Storage.Faster.Abis;

public sealed class AbiIdSerializer : BinaryObjectSerializer<AbiId>
{
    public override void Deserialize(out AbiId obj)
    {
        obj = new AbiId(reader.ReadUInt64());
    }

    public override void Serialize(ref AbiId obj)
    {
        writer.Write(obj.Id);
    }
}