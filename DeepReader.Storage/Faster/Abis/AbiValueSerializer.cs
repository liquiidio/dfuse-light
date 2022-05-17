using DeepReader.Types.FlattenedTypes;
using FASTER.core;
using System.Runtime.Serialization.Formatters.Binary;

namespace DeepReader.Storage.Faster.Abis;

public class AbiValueSerializer : BinaryObjectSerializer<AbiCacheItem>
{
    public override void Deserialize(out AbiCacheItem obj)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        obj = (AbiCacheItem)formatter.Deserialize(reader.BaseStream);
    }

    public override void Serialize(ref AbiCacheItem obj)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(writer.BaseStream, obj);
    }
}