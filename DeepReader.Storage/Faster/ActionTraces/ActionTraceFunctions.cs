using DeepReader.Types.StorageTypes;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.ActionTraces;

public sealed class ActionTraceFunctions : FunctionsBase<ulong, ActionTrace, ActionTraceInput,ActionTraceOutput, ActionTraceContext>
{
    public override bool ConcurrentReader(ref ulong id, ref ActionTraceInput input, ref ActionTrace value, ref ActionTraceOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }

    public override void CheckpointCompletionCallback(int sessionId, string sessionName, CommitPoint commitPoint)
    {
        Log.Information("Session {0} reports persistence until {1}", sessionName, commitPoint.UntilSerialNo);
    }

    public override void ReadCompletionCallback(ref ulong id, ref ActionTraceInput input, ref ActionTraceOutput output, ActionTraceContext ctx, Status status, RecordMetadata recordMetadata)
    {
        if (ctx.Type == 0)
        {
            if (output.Value.GlobalSequence != id)
                Log.Error( new Exception("Read error!, ActionTraceId.BinarySequencens unequal"),"");
        }
        else
        {
            long ticks = DateTime.Now.Ticks - ctx.Ticks;

            if (status.Found)
                Log.Information("Async: Value not found, latency = {0}ms", new TimeSpan(ticks).TotalMilliseconds);

            if (output.Value.GlobalSequence != id)
                Log.Information("Async: Incorrect value {0} found, latency = {1}ms", output.Value.GlobalSequence,
                    new TimeSpan(ticks).TotalMilliseconds);
            else
                Log.Information("Async: Correct value {0} found, latency = {1}ms", output.Value.GlobalSequence,
                    new TimeSpan(ticks).TotalMilliseconds);
        }
    }

    public override bool SingleReader(ref ulong id, ref ActionTraceInput input, ref ActionTrace value, ref ActionTraceOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }
}