using DeepReader.Storage.Faster.Stores.Abis.Standalone;
using DeepReader.Storage.Options;
using FASTER.common;
using FASTER.core;
using FASTER.server;
using HotChocolate.Subscriptions;

namespace DeepReader.Storage.Faster.Stores.Abis.Server
{
    internal class AbiStoreServer : AbiStore
    {
        readonly ServerOptions _serverOptions;
        readonly IFasterServer _server;
        readonly AbiFasterKvProvider _provider;
        readonly SubscribeKVBroker<ulong, AbiCacheItem, AbiInput, IKeyInputSerializer<ulong, AbiInput>> _kvBroker;
        readonly SubscribeBroker<ulong, AbiCacheItem, IKeySerializer<ulong>> _broker;
        readonly LogSettings _logSettings;

        public AbiStoreServer(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            _serverOptions = new ServerOptions()
            {
                Port = 5004,
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
                _kvBroker = new SubscribeKVBroker<ulong, AbiCacheItem, AbiInput, IKeyInputSerializer<ulong, AbiInput>>(new AbiKeyInputSerializer(), null, _serverOptions.PubSubPageSizeBytes(), true);
                _broker = new SubscribeBroker<ulong, AbiCacheItem, IKeySerializer<ulong>>(new AbiServerKeySerializer(), null, _serverOptions.PubSubPageSizeBytes(), true);
            }

            // Create session provider for VarLen
            _provider = new AbiFasterKvProvider(Store, new AbiServerSerializer(), _kvBroker, _broker, _serverOptions.Recover);

            _server = new FasterServerTcp(_serverOptions.Address, _serverOptions.Port);
            _server.Register(WireFormat.DefaultVarLenKV, _provider);
            _server.Register(WireFormat.WebSocket, _provider);
        }
    }
}