using DeepReader.Types.FlattenedTypes;
using FASTER.core;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace DeepReader.Storage.Faster.Abis;

public class AbiValueSerializer : BinaryObjectSerializer<AbiCacheItem>
{
    public override void Deserialize(out AbiCacheItem obj)
    {
        // TODO
        //var assembly = Assembly.Load(reader.ReadBytes(0));
        //BinaryFormatter formatter = new BinaryFormatter();
        //obj = (AbiCacheItem)formatter.Deserialize(reader.BaseStream);
        obj = new AbiCacheItem();
    }

    public override void Serialize(ref AbiCacheItem obj)
    {
        // TODO
        writer.Write(0);
        //var generator = new Lokad.ILPack.AssemblyGenerator();
        //generator.GenerateAssemblyBytes()
        //BinaryFormatter formatter = new BinaryFormatter();
        //formatter.Serialize(writer.BaseStream, obj);
    }
}