using System.Reflection;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.Stores.Abis.Standalone;

public sealed class AbiFunctions : FunctionsBase<ulong, AbiCacheItem, AbiInput, AbiOutput, AbiContext>
{
    public override bool ConcurrentReader(ref ulong id, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }

    public override void CheckpointCompletionCallback(int sessionId, string sessionName, CommitPoint commitPoint)
    {
        Log.Information("Session {0} reports persistence until {1}", sessionName, commitPoint.UntilSerialNo);
    }

    //public override void ReadCompletionCallback(ref ulong id, ref AbiInput input, ref AbiOutput output, AbiContext ctx, Status status, RecordMetadata recordMetadata)
    //{
    //    if (ctx.Type == 0)
    //    {
    //        // TODO
    //        //if (output.Value.Id != id.Id)
    //        //    Log.Error(new Exception("Read error!, Abi not found"), "");
    //    }
    //    else
    //    {
    //        long ticks = DateTime.Now.Ticks - ctx.Ticks;

    //        if (status.Found)
    //            Log.Information("Async: Value not found, latency = {0}ms", new TimeSpan(ticks).TotalMilliseconds);

    //        if (output.Value.Id != id)
    //            Log.Information("Async: Incorrect value {0} found, latency = {1}ms", output.Value.Id,
    //                new TimeSpan(ticks).TotalMilliseconds);
    //        else
    //            Log.Information("Async: Correct value {0} found, latency = {1}ms", output.Value.Id,
    //                new TimeSpan(ticks).TotalMilliseconds);
    //    }
    //}

    public override bool SingleReader(ref ulong id, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }

    public override bool InitialUpdater(ref ulong key, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput output, ref RMWInfo rmwInfo)
    {
        value = new AbiCacheItem(input.Id)
        {
            AbiVersions =
            {
                [input.GlobalSequence] = new AssemblyWrapper(input.Assembly)
            }
        };

        output.Value = value;

        return true;
    }

    public override bool InPlaceUpdater(ref ulong key, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput output, ref RMWInfo rmwInfo)
    {
        value.AbiVersions[input.GlobalSequence] = new AssemblyWrapper(input.Assembly);

        output.Value = value;

        return true;
    }

    public override bool CopyUpdater(ref ulong key, ref AbiInput input, ref AbiCacheItem oldValue, ref AbiCacheItem newValue, ref AbiOutput output, ref RMWInfo rmwInfo)
    {
        if (oldValue != null)
            newValue = oldValue;
        else
            newValue = new AbiCacheItem(input.Id);

        newValue.AbiVersions[input.GlobalSequence] = new AssemblyWrapper(input.Assembly);
        output.Value = newValue;

        return true;
    }

}


public class AbiCacheItem
{
    public ulong Id;
    public SortedDictionary<ulong, AssemblyWrapper> AbiVersions = new();

    public AbiCacheItem(ulong id)
    {
        Id = id;
    }
}

public class AssemblyWrapper
{
    private byte[]? _binary;

    private Assembly? _assembly;

    public Assembly Assembly => _assembly ??= (_binary != null ? Assembly.Load(_binary) : null);

    public byte[] Binary => _binary ??= AssemblyToByteArray();

    public AssemblyWrapper(Assembly assembly)
    {
        _assembly = assembly;
    }

    public AssemblyWrapper(byte[] binary)
    {
        _binary = binary;
    }

    byte[] AssemblyToByteArray()
    {
        var generator = new Lokad.ILPack.AssemblyGenerator();
        return generator.GenerateAssemblyBytes(_assembly);
    }

    public static implicit operator Assembly(AssemblyWrapper wrapper)
    {
        return wrapper.Assembly;
    }
}