using DeepReader.Types.FlattenedTypes;
using FASTER.core;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace DeepReader.Storage.Faster.Abis;

public class AbiValueSerializer : BinaryObjectSerializer<AbiCacheItem>
{
    public override void Deserialize(out AbiCacheItem obj)
    {
        obj = new AbiCacheItem();
        obj.Id = reader.ReadUInt64();
        obj.AbiVersions[reader.ReadUInt64()] = new AssemblyWrapper(reader.ReadBytes(reader.Read7BitEncodedInt()));
    }

    public override void Serialize(ref AbiCacheItem obj)
    {
        writer.Write(obj.Id);
        foreach(var item in obj.AbiVersions)
        {
            writer.Write(item.Key);
            writer.Write7BitEncodedInt(item.Value.Binary.Length);
            writer.Write(item.Value.Binary);
        }
    }
}