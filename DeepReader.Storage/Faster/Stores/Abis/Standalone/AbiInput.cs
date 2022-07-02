using System.Reflection;

namespace DeepReader.Storage.Faster.Stores.Abis.Standalone;

public struct AbiInput
{
    public ulong Id;
    public ulong GlobalSequence;
    public Assembly Assembly;

    public AbiInput(ulong id, ulong globalSequence, Assembly assembly)
    {
        Id = id;
        GlobalSequence = globalSequence;
        Assembly = assembly;
    }
}