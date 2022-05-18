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
        var abiCount = reader.Read7BitEncodedInt();
        for(int i = 0; i < abiCount; i++)
        {
            var key = reader.ReadUInt64();
            var bytes = reader.Read7BitEncodedInt();
            obj.AbiVersions[key] = new AssemblyWrapper(reader.ReadBytes(bytes));
        }
    }

    public override void Serialize(ref AbiCacheItem obj)
    {
        writer.Write(obj.Id);
        writer.Write7BitEncodedInt(obj.AbiVersions.Count);
        foreach(var item in obj.AbiVersions)
        {
            writer.Write(item.Key);
            writer.Write7BitEncodedInt(item.Value.Binary.Length);
            writer.Write(item.Value.Binary);
        }
    }
}