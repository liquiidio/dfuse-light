using FASTER.server;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;
using HotChocolate.Subscriptions;
using DeepReader.Storage.Faster.ActionTraces.Base;
using DeepReader.Storage.Faster.ActionTraces.Standalone;

namespace DeepReader.Storage.Faster.ActionTraces.Server
{
    internal class ActionTraceStoreServer : ActionTraceStore
    {
        readonly ServerOptions _serverOptions;
        readonly IFasterServer server;
        readonly ActionTraceFasterKVProvider provider;
        readonly SubscribeKVBroker<ulong, ActionTrace, ActionTraceInput, IKeyInputSerializer<ulong, ActionTraceInput>> kvBroker;
        readonly SubscribeBroker<ulong, ActionTrace, IKeySerializer<ulong>> broker;
        readonly LogSettings logSettings;


        private readonly VarLenServer _server;

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

            _server = new VarLenServer(_serverOptions);

            if (!_serverOptions.DisablePubSub)
            {
                kvBroker = new SubscribeKVBroker<ulong, ActionTrace, ActionTraceInput, IKeyInputSerializer<ulong, ActionTraceInput>>(new ActionTraceKeyInputSerializer(), null, _serverOptions.PubSubPageSizeBytes(), true);
                broker = new SubscribeBroker<ulong, ActionTrace, IKeySerializer<ulong>>(new ActionTraceServerKeySerializer(), null, _serverOptions.PubSubPageSizeBytes(), true);
            }

            // Create session provider for VarLen
            provider = new ActionTraceFasterKVProvider(_store, new ActionTraceServerSerializer(), kvBroker, broker, _serverOptions.Recover);

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