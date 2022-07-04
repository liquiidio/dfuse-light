using DeepReader.Storage.Faster.Options;
using DeepReader.Storage.Faster.StoreBase.Server;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.server;
using HotChocolate.Subscriptions;

namespace DeepReader.Storage.Faster.Stores.ActionTraces
{
    internal class ActionTraceStoreServer : ActionTraceStore
    {
        private FasterServerOptions ServerOptions => (FasterServerOptions)Options;

        readonly IFasterServer _server;
        readonly ServerKVProvider<UlongKey, ulong, ActionTrace> _provider;
        readonly SubscribeKVBroker<ulong, ActionTrace, ActionTrace, IKeyInputSerializer<ulong, ActionTrace>> _kvBroker;
        readonly SubscribeBroker<ulong, ActionTrace, IKeySerializer<ulong>> _broker;

        public ActionTraceStoreServer(IFasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            if (!ServerOptions.DisablePubSub)
            {
                _kvBroker = new SubscribeKVBroker<ulong, ActionTrace, ActionTrace, IKeyInputSerializer<ulong, ActionTrace>>(
                    new ServerKeyInputSerializer<UlongKey, ulong, ActionTrace>(), null, ServerOptions.PubSubPageSizeBytes, true);
                _broker = new SubscribeBroker<ulong, ActionTrace, IKeySerializer<ulong>>(
                    new ServerKeyInputSerializer<UlongKey, ulong, ActionTrace>(), null, ServerOptions.PubSubPageSizeBytes, true);
            }

            // Create session provider for VarLen
            _provider = new ServerKVProvider<UlongKey, ulong, ActionTrace>(Store, new ServerSerializer<UlongKey, ulong, ActionTrace>(), _kvBroker, _broker);

            _server = new FasterServerTcp(ServerOptions.IpAddress, ServerOptions.ActionStorePort);
            _server.Register(WireFormat.DefaultVarLenKV, _provider);
            _server.Register(WireFormat.WebSocket, _provider);
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