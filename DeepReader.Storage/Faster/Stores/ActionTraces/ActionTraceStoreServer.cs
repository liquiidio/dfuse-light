using FASTER.server;
using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;
using HotChocolate.Subscriptions;
using DeepReader.Storage.Faster.Base;
using DeepReader.Storage.Faster.Test.Server;

namespace DeepReader.Storage.Faster.ActionTraces
{
    internal class ActionTraceStoreServer : ActionTraceStore
    {
        readonly ServerOptions _serverOptions;
        readonly IFasterServer server;
        readonly ServerKVProvider<UlongKey, ulong, ActionTrace> provider;
        readonly SubscribeKVBroker<ulong, ActionTrace, ulong, IKeyInputSerializer<ulong, ulong>> kvBroker;
        readonly SubscribeBroker<ulong, ActionTrace, IKeySerializer<ulong>> broker;
        readonly LogSettings logSettings;

        public ActionTraceStoreServer(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            // We already have the Store and everything set up in base-class ActionTraceStore


            _serverOptions = new ServerOptions()
            {
                Port = 5005,
                Address = "127.0.0.1",
                MemorySize = "16g",
                PageSize = "32m",
                SegmentSize = "1g",
                IndexSize = "8g",
                EnableStorageTier = false,
                LogDir = null,
                CheckpointDir = null,
                Recover = false,
                DisablePubSub = false,
                PubSubPageSize = "4k"
            };

            if (!_serverOptions.DisablePubSub)
            {
                kvBroker = new SubscribeKVBroker<ulong, ActionTrace, ulong, IKeyInputSerializer<ulong, ulong>>(
                    new ServerKeyInputSerializer<UlongKey, ulong>(), null, _serverOptions.PubSubPageSizeBytes(), true);
                broker = new SubscribeBroker<ulong, ActionTrace, IKeySerializer<ulong>>(
                    new ServerKeyInputSerializer<UlongKey, ulong>(), null, _serverOptions.PubSubPageSizeBytes(), true);
            }

            // Create session provider for VarLen
            provider = new ServerKVProvider<UlongKey, ulong, ActionTrace>(_store, new ServerSerializer<UlongKey, ulong, ActionTrace>(), kvBroker, broker, _serverOptions.Recover);

            server = new FasterServerTcp(_serverOptions.Address, _serverOptions.Port);
            server.Register(WireFormat.DefaultVarLenKV, provider);
            server.Register(WireFormat.WebSocket, provider);
        }

        //protected override void CollectObservableMetrics(object? sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //public override Task<Status> WriteActionTrace(ActionTrace actionTrace)
        //{
        //    throw new NotImplementedException();
        //}

        //public override Task<(bool, ActionTrace)> TryGetActionTraceById(ulong globalSequence)
        //{
        //    throw new NotImplementedException();
        //}
    }
}