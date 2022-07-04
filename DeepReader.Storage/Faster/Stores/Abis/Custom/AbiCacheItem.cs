using System.Reflection;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;

namespace DeepReader.Storage.Faster.Stores.Abis.Custom;

public class AbiCacheItem : IFasterSerializable<AbiCacheItem>
{
    public ulong Id;
    public SortedDictionary<ulong, AssemblyWrapper> AbiVersions = new();

    public AbiCacheItem(ulong id)
    {
        Id = id;
    }

    public AbiCacheItem(ulong id, ulong globalSequence, Assembly assembly)
    {
        Id = id;
        AbiVersions[globalSequence] = new AssemblyWrapper(assembly);
    }

    public static AbiCacheItem ReadFromFaster(IBufferReader reader, bool fromPool = true)
    {
        var obj = new AbiCacheItem(reader.ReadUInt64());
        var abiCount = reader.Read7BitEncodedInt();
        for (int i = 0; i < abiCount; i++)
        {
            var key = reader.ReadUInt64();
            var length = reader.Read7BitEncodedInt();
            obj.AbiVersions[key] = new AssemblyWrapper(reader.ReadBytes(length));
        }
        return obj;
    }

    public void WriteToFaster(IBufferWriter writer)
    {
        writer.Write(Id);
        var abiVersions = AbiVersions.ToArray();
        writer.Write7BitEncodedInt(abiVersions.Length);
        foreach (var item in abiVersions)
        {
            writer.Write(item.Key);
            writer.Write7BitEncodedInt(item.Value.Binary.Length);
            writer.Write(item.Value.Binary);
        }
    }
}