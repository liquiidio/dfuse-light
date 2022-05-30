using DeepReader.Types.StorageTypes;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.Blocks;

public sealed class BlockFunctions : FunctionsBase<long, Block, BlockInput, BlockOutput, BlockContext>
{
    public override bool ConcurrentReader(ref long id, ref BlockInput input, ref Block value, ref BlockOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }

    public override void CheckpointCompletionCallback(int sessionId, string sessionName, CommitPoint commitPoint)
    {
        Log.Information("Session {0} reports persistence until {1}", sessionName, commitPoint.UntilSerialNo);
    }

    //public override void ReadCompletionCallback(ref long id, ref BlockInput input, ref BlockOutput output, BlockContext ctx, Status status, RecordMetadata recordMetadata)
    //{
    //    if (ctx.Type == 0)
    //    {
    //        if (output.Value == null || id == null || output.Value.Number != id)
    //            Log.Error(new Exception("Read error! BlockIds unequal"),"");
    //    }
    //    else
    //    {
    //        long ticks = DateTime.Now.Ticks - ctx.Ticks;

    //        if (status.Found)
    //            Log.Information("Async: Value not found, latency = {0}ms", new TimeSpan(ticks).TotalMilliseconds);

    //        if (output.Value.Number != id)
    //            Log.Information("Async: Incorrect value {0} found, latency = {1}ms", output.Value.Number, new TimeSpan(ticks).TotalMilliseconds);
    //        else
    //            Log.Information("Async: Correct value {0} found, latency = {1}ms", output.Value.Number, new TimeSpan(ticks).TotalMilliseconds);
    //    }
    //}

    public override bool SingleReader(ref long id, ref BlockInput input, ref Block value, ref BlockOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }
}