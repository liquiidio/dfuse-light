using System.Collections.Concurrent;
using System.Reflection;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Helpers;
using Serilog;

namespace DeepReader.AssemblyGenerator;

public class AbiTypeCacheEntry
{
    public ConcurrentDictionary<string, string> AbiTypeTypes;
    public ConcurrentDictionary<string, Type> AbiStructTypes;
    public ConcurrentDictionary<string, string> AbiActionTypes;
    public ConcurrentDictionary<string, string> AbiTableTypes;

    public AbiTypeCacheEntry()
    {
        AbiTypeTypes = new ConcurrentDictionary<string, string>();
        AbiStructTypes = new ConcurrentDictionary<string, Type>();
        AbiActionTypes = new ConcurrentDictionary<string, string>();
        AbiTableTypes = new ConcurrentDictionary<string, string>();
    }

    public AbiTypeCacheEntry(ConcurrentDictionary<string, string> abiTypeTypes, ConcurrentDictionary<string, Type> abiStructTypes,
        ConcurrentDictionary<string, string> abiActionTypes, ConcurrentDictionary<string, string> abiTableTypes)
    {
        AbiStructTypes = abiStructTypes;
        AbiActionTypes = abiActionTypes;
        AbiTableTypes = abiTableTypes;
        AbiTypeTypes = abiTypeTypes;
    }

    public bool TryGetTypeType(string name, out Type type)
    {
        if (AbiTypeTypes.TryGetValue(name, out var typeName))
            if (AbiStructTypes.TryGetValue(typeName, out type!))
                return true;
        return RuntimeAssemblyGenerator.EosTypes.TryGetValue(name, out type!);
    }

    public bool TryGetStructType(string name, out Type type)
    {
        return AbiStructTypes.TryGetValue(name, out type!);
    }

    public bool TryGetActionType(string name, out Type type)
    {
        if (AbiActionTypes.TryGetValue(name, out var actionTypeName))
            return AbiStructTypes.TryGetValue(actionTypeName, out type!);
        return RuntimeAssemblyGenerator.EosTypes.TryGetValue(name, out type!);
    }

    public bool TryGetTableType(string name, out Type type)
    {
        if (AbiTableTypes.TryGetValue(name, out var tableTypeName))
            return AbiStructTypes.TryGetValue(tableTypeName, out type!);
        return RuntimeAssemblyGenerator.EosTypes.TryGetValue(name, out type!);
    }
}

public static class AssemblyCache
{
    public static ConcurrentDictionary<ulong, SortedDictionary<uint,AbiTypeCacheEntry>> ContractAssemblyCache = new();

    public static void LoadAssemblies()
    {
        foreach (var directory in Directory.GetDirectories(RuntimeAssemblyGenerator.AssemblyPath))
        {
            var abiPaths = Directory.GetFiles(RuntimeAssemblyGenerator.AssemblyPath, "*.abi.bin").Select(a => a.Replace(".abi.bin", ""));// TODO substring with index
            foreach (var dllPath in Directory.GetFiles(RuntimeAssemblyGenerator.AssemblyPath, "*.dll"))
            {
                var assembly = Assembly.LoadFile(dllPath);
                //var assemblyTypes = new Dictionary<string, Type>();

                //foreach (var assemblyType in assembly.GetTypes())
                //{
                //    assemblyTypes.Add(assemblyType.Name, assemblyType);
                //}

                // TODO

                var contractName = assembly.GetName().Name ?? assembly.GetName().FullName;
                if (!ContractAssemblyCache.ContainsKey(SerializationHelper.ConvertNameToLong(contractName)))
                    Log.Information("duplicate Assembly found");
            }
        }
    }

    public static void UpsertAbiTypeCacheEntry(Name contractName, AbiTypeCacheEntry abiTypeCacheEntry, uint blockNum)
    {
        if(ContractAssemblyCache.ContainsKey(contractName))
            ContractAssemblyCache[contractName].Add(blockNum, abiTypeCacheEntry);
        else
            ContractAssemblyCache[contractName] = new SortedDictionary<uint, AbiTypeCacheEntry>(){{blockNum,abiTypeCacheEntry}};
    }

}