using DeepReader.Types.StorageTypes;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.ActionTraces.Standalone;

public sealed class ActionTraceFunctions : FunctionsBase<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceContext>
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

    public override bool SingleReader(ref ulong id, ref ActionTraceInput input, ref ActionTrace value, ref ActionTraceOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }
}