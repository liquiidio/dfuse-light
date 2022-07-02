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
        readonly IFasterServer server;
        readonly AbiFasterKVProvider provider;
        readonly SubscribeKVBroker<ulong, AbiCacheItem, AbiInput, IKeyInputSerializer<ulong, AbiInput>> kvBroker;
        readonly SubscribeBroker<ulong, AbiCacheItem, IKeySerializer<ulong>> broker;
        readonly LogSettings logSettings;

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
                kvBroker = new SubscribeKVBroker<ulong, AbiCacheItem, AbiInput, IKeyInputSerializer<ulong, AbiInput>>(new AbiKeyInputSerializer(), null, _serverOptions.PubSubPageSizeBytes(), true);
                broker = new SubscribeBroker<ulong, AbiCacheItem, IKeySerializer<ulong>>(new AbiServerKeySerializer(), null, _serverOptions.PubSubPageSizeBytes(), true);
            }

            // Create session provider for VarLen
            provider = new AbiFasterKVProvider(_store, new AbiServerSerializer(), kvBroker, broker, _serverOptions.Recover);

            server = new FasterServerTcp(_serverOptions.Address, _serverOptions.Port);
            server.Register(WireFormat.DefaultVarLenKV, provider);
            server.Register(WireFormat.WebSocket, provider);
        }
    }
}