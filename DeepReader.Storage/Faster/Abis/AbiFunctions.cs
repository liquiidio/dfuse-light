using DeepReader.Types.FlattenedTypes;
using FASTER.core;
using Serilog;
using System.Reflection;

namespace DeepReader.Storage.Faster.Abis;

public sealed class AbiFunctions : FunctionsBase<AbiId, AbiCacheItem, AbiInput, AbiOutput, AbiContext>
{
    public override bool ConcurrentReader(ref AbiId id, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }

    public override void CheckpointCompletionCallback(int sessionId, string sessionName, CommitPoint commitPoint)
    {
        Log.Information("Session {0} reports persistence until {1}", sessionName, commitPoint.UntilSerialNo);
    }

    public override void ReadCompletionCallback(ref AbiId id, ref AbiInput input, ref AbiOutput output, AbiContext ctx, Status status, RecordMetadata recordMetadata)
    {
        if (ctx.Type == 0)
        {
            if (output.Value?.Id != id.Id)
                Log.Error( new Exception("Read error!, Abi not found"),"");
        }
        else
        {
            long ticks = DateTime.Now.Ticks - ctx.Ticks;

            if (status.Found)
                Log.Information("Async: Value not found, latency = {0}ms", new TimeSpan(ticks).TotalMilliseconds);

            if (output.Value.Id != id.Id)
                Log.Information("Async: Incorrect value {0} found, latency = {1}ms", output.Value.Id,
                    new TimeSpan(ticks).TotalMilliseconds);
            else
                Log.Information("Async: Correct value {0} found, latency = {1}ms", output.Value.Id,
                    new TimeSpan(ticks).TotalMilliseconds);
        }
    }

    public override bool SingleReader(ref AbiId id, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }

    public override bool InitialUpdater(ref AbiId key, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput output, ref RMWInfo rmwInfo)
    {
        value = new AbiCacheItem();
        value.AbiVersions[input.GlobalSequence] = new AssemblyWrapper(input.Assembly);
        value.Id = input.Id;

        output.Value = value;
        
        return true;
    }

    public override bool InPlaceUpdater(ref AbiId key, ref AbiInput input, ref AbiCacheItem value, ref AbiOutput output, ref RMWInfo rmwInfo)
    {
        value.AbiVersions[input.GlobalSequence] = new AssemblyWrapper(input.Assembly);

        output.Value = value;

        return true;
    }

    public override bool CopyUpdater(ref AbiId key, ref AbiInput input, ref AbiCacheItem oldValue, ref AbiCacheItem newValue, ref AbiOutput output, ref RMWInfo rmwInfo)
    {
        if (oldValue == null)
            Log.Information("test");

        if (input.Assembly == null)
            Log.Information("Test");

        if (newValue != null)
            Log.Information("Test");

        //oldValue.AbiVersions[input.GlobalSequence] = new AssemblyWrapper(input.Assembly);
        newValue = oldValue;
        newValue.AbiVersions[input.GlobalSequence] = new AssemblyWrapper(input.Assembly);

        return true;
    }

}


public class AbiCacheItem
{
    public ulong Id;
    public SortedDictionary<ulong, AssemblyWrapper> AbiVersions = new();
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