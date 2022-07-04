using DeepReader.Storage.Faster.StoreBase.Standalone;
using FASTER.core;

namespace DeepReader.Storage.Faster.Stores.Abis.Custom;

public sealed class StandaloneAbiFunctions : StandaloneFunctions<ulong, AbiCacheItem>
{
    public override bool InitialUpdater(ref ulong key, ref AbiCacheItem input, ref AbiCacheItem value, ref AbiCacheItem output,
        ref RMWInfo rmwInfo)
    {
        value = new AbiCacheItem(input.Id)
        {
            AbiVersions =
            {
                [input.AbiVersions.First().Key] = new AssemblyWrapper(input.AbiVersions.First().Value)
            }
        };

        output = value;

        return true;
    }

    public override bool InPlaceUpdater(ref ulong key, ref AbiCacheItem input, ref AbiCacheItem value, ref AbiCacheItem output,
        ref RMWInfo rmwInfo)
    {
        value.AbiVersions[input.AbiVersions.First().Key] = new AssemblyWrapper(input.AbiVersions.First().Value);

        output = value;

        return true;
    }

    public override bool CopyUpdater(ref ulong key, ref AbiCacheItem input, ref AbiCacheItem oldValue, ref AbiCacheItem newValue,
        ref AbiCacheItem output, ref RMWInfo rmwInfo)
    {
        if (oldValue != null)
            newValue = oldValue;
        else
            newValue = new AbiCacheItem(input.Id);

        newValue.AbiVersions[input.AbiVersions.First().Key] = new AssemblyWrapper(input.AbiVersions.First().Value);
        output = newValue;

        return true;
    }
}