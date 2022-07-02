using FASTER.core;

namespace DeepReader.Storage.Faster.Abis.Standalone;

public sealed class AbiValueSerializer : BinaryObjectSerializer<AbiCacheItem>
{
    public override void Deserialize(out AbiCacheItem obj)
    {
        obj = new AbiCacheItem(reader.ReadUInt64());
        var abiCount = reader.Read7BitEncodedInt();
        for (int i = 0; i < abiCount; i++)
        {
            var key = reader.ReadUInt64();
            var length = reader.Read7BitEncodedInt();
            obj.AbiVersions[key] = new AssemblyWrapper(reader.ReadBytes(length));
        }
    }

    public override void Serialize(ref AbiCacheItem obj)
    {
        writer.Write(obj.Id);
        var abiVerions = obj.AbiVersions.ToArray();
        writer.Write7BitEncodedInt(abiVerions.Length);
        foreach (var item in abiVerions)
        {
            writer.Write(item.Key);
            writer.Write7BitEncodedInt(item.Value.Binary.Length);
            writer.Write(item.Value.Binary);
        }
    }
}