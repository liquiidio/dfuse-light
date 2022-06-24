using DeepReader.Storage.Faster.ActionTraces.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.client;

namespace DeepReader.Storage.Faster.ActionTraces.Client;

public sealed class ActionTraceClientFunctions : ICallbackFunctions<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceContext>
{
    // @Haron
    // not sure which Methods here really need a body
    // Feeling is none of them needs a body
    public void ReadCompletionCallback(ref ulong key, ref ActionTraceInput input, ref ActionTraceOutput output, ActionTraceContext ctx, Status status)
    {
    }

    public void UpsertCompletionCallback(ref ulong key, ref ActionTrace value, ActionTraceContext ctx)
    {
    }

    public void RMWCompletionCallback(ref ulong key, ref ActionTraceInput input, ref ActionTraceOutput output, ActionTraceContext ctx, Status status)
    {
    }

    public void DeleteCompletionCallback(ref ulong key, ActionTraceContext ctx)
    {
    }

    public void SubscribeKVCallback(ref ulong key, ref ActionTraceInput input, ref ActionTraceOutput output, ActionTraceContext ctx, Status status)
    {
    }

    public void PublishCompletionCallback(ref ulong key, ref ActionTrace value, ActionTraceContext ctx)
    {
    }

    public void SubscribeCallback(ref ulong key, ref ActionTrace value, ActionTraceContext ctx)
    {
    }
}