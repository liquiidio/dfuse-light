using DeepReader.Storage.Faster.StoreBase.Server;
using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.server;
using HotChocolate.Subscriptions;

namespace DeepReader.Storage.Faster.Stores.ActionTraces
{
    internal class ActionTraceStoreServer : ActionTraceStore
    {
        readonly ServerOptions _serverOptions;
        readonly IFasterServer _server;
        readonly ServerKvProvider<UlongKey, ulong, ActionTrace> _provider;
        readonly SubscribeKVBroker<ulong, ActionTrace, ulong, IKeyInputSerializer<ulong, ulong>> _kvBroker;
        readonly SubscribeBroker<ulong, ActionTrace, IKeySerializer<ulong>> _broker;

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
                _kvBroker = new SubscribeKVBroker<ulong, ActionTrace, ulong, IKeyInputSerializer<ulong, ulong>>(
                    new ServerKeyInputSerializer<UlongKey, ulong>(), null, _serverOptions.PubSubPageSizeBytes(), true);
                _broker = new SubscribeBroker<ulong, ActionTrace, IKeySerializer<ulong>>(
                    new ServerKeyInputSerializer<UlongKey, ulong>(), null, _serverOptions.PubSubPageSizeBytes(), true);
            }

            // Create session provider for VarLen
            _provider = new ServerKvProvider<UlongKey, ulong, ActionTrace>(Store, new ServerSerializer<UlongKey, ulong, ActionTrace>(), _kvBroker, _broker, _serverOptions.Recover);

            _server = new FasterServerTcp(_serverOptions.Address, _serverOptions.Port);
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