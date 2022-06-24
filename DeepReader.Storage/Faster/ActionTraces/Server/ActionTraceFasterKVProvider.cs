using DeepReader.Storage.Faster.ActionTraces.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;
using FASTER.server;

namespace DeepReader.Storage.Faster.ActionTraces.Server;

/// <summary>
/// Session provider for FasterKV store based on
/// [K, V, I, O, C] = [ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceServerFunctions, ActionTraceServerSerializer]
/// </summary>
public sealed class ActionTraceFasterKVProvider : FasterKVProviderBase<ulong, ActionTrace, ActionTraceInput,
    ActionTraceOutput, ActionTraceServerFunctions, ActionTraceServerSerializer>
{
    /// <summary>
    /// Create TODO default SpanByte FasterKV backend with SpanByteFunctionsForServer&lt;long&gt; as functions, and SpanByteServerSerializer as parameter serializer
    /// </summary>
    /// <param name="store"></param>
    /// <param name="kvBroker"></param>
    /// <param name="broker"></param>
    /// <param name="recoverStore"></param>
    /// <param name="maxSizeSettings"></param>
    public ActionTraceFasterKVProvider(FasterKV<ulong, ActionTrace> store, ActionTraceServerSerializer serializer,
        SubscribeKVBroker<ulong, ActionTrace, ActionTraceInput, IKeyInputSerializer<ulong, ActionTraceInput>> kvBroker = null, 
        SubscribeBroker<ulong, ActionTrace, IKeySerializer<ulong>> broker = null, bool recoverStore = false,
        MaxSizeSettings maxSizeSettings = null) 
        : base(store, serializer, kvBroker, broker, recoverStore, maxSizeSettings)
    {
    }

    /// <inheritdoc />
    // public override SpanByteFunctionsForServer<long> GetFunctions() => new();

    /// <inheritdoc />
    public override ActionTraceServerFunctions GetFunctions() => new();
}