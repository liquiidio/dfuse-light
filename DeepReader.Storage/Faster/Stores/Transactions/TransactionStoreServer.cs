using DeepReader.Storage.Faster.StoreBase.Server;
using DeepReader.Storage.Options;
using DeepReader.Types.Eosio.Chain;
using FASTER.common;
using FASTER.core;
using FASTER.server;
using HotChocolate.Subscriptions;
using TransactionTrace = DeepReader.Types.StorageTypes.TransactionTrace;

namespace DeepReader.Storage.Faster.Stores.Transactions
{
    public class TransactionStoreServer : TransactionStore
    {
        readonly ServerOptions _serverOptions;
        readonly IFasterServer _server;
        readonly ServerKvProvider<TransactionId, TransactionId, TransactionTrace> _provider;
        readonly SubscribeKVBroker<TransactionId, TransactionTrace, TransactionId, IKeyInputSerializer<TransactionId, TransactionId>> _kvBroker;
        readonly SubscribeBroker<TransactionId, TransactionTrace, IKeySerializer<TransactionId>> _broker;
        readonly LogSettings _logSettings;

        public TransactionStoreServer(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            // TODO, we can probably remove these ServerOptions and use our own ServerOptions-Class instead
            // ServerOptions seems to only make sense with GenericServer but not with our approach
            _serverOptions = new ServerOptions()
            {
                Port = 5003,
                Address = "127.0.0.1",
                MemorySize = "16g", // only used in GenericServer and VarLenServer for underlying store
                PageSize = "32m", // only used in GenericServer and VarLenServer for underlying store
                SegmentSize = "1g", // only used in GenericServer and VarLenServer for underlying store
                IndexSize = "8g", // only used in GenericServer and VarLenServer for underlying store
                EnableStorageTier = false, // only used in GenericServer and VarLenServer for underlying store
                LogDir = null, // only used in GenericServer and VarLenServer for underlying store
                CheckpointDir = null,  // only used in GenericServer and VarLenServer for underlying store
                Recover = false, // we are already recovering in base
                DisablePubSub = false,
                PubSubPageSize = "4k"
            };

            if (!_serverOptions.DisablePubSub)
            {
                _kvBroker =
                    new SubscribeKVBroker<TransactionId, TransactionTrace, TransactionId,
                        IKeyInputSerializer<TransactionId, TransactionId>>(new ServerKeyInputSerializer<TransactionId, TransactionId>(), null,
                        _serverOptions.PubSubPageSizeBytes(), true);
                _broker = new SubscribeBroker<TransactionId, TransactionTrace, IKeySerializer<TransactionId>>(
                        new ServerKeyInputSerializer<TransactionId, TransactionId>(), null,
                        _serverOptions.PubSubPageSizeBytes(), true);
            }

            _provider = new ServerKvProvider<TransactionId, TransactionId, TransactionTrace>(Store, new ServerSerializer<TransactionId, TransactionId, TransactionTrace>(), _kvBroker, _broker, _serverOptions.Recover);

            _server = new FasterServerTcp(_serverOptions.Address, _serverOptions.Port);
            _server.Register(WireFormat.DefaultVarLenKV, _provider);
            _server.Register(WireFormat.WebSocket, _provider);

            _server.Start();
        }
    }
}