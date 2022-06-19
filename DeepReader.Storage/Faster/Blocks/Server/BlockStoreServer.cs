using DeepReader.Storage.Faster.Blocks.Standalone;
using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;
using FASTER.server;
using HotChocolate.Subscriptions;

namespace DeepReader.Storage.Faster.Blocks.Server
{
    public class BlockStoreServer : BlockStore
    {
        readonly ServerOptions _serverOptions;
        readonly IFasterServer server;
        readonly BlockFasterKVProvider provider;
        readonly SubscribeKVBroker<long, Block, BlockInput, IKeyInputSerializer<long, BlockInput>> kvBroker;
        readonly SubscribeBroker<long, Block, IKeySerializer<long>> broker;
        readonly LogSettings logSettings;


        public BlockStoreServer(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            _serverOptions = new ServerOptions()
            {
                Port = 5002,
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
                kvBroker = new SubscribeKVBroker<long, Block, BlockInput, IKeyInputSerializer<long, BlockInput>>(new BlockKeyInputSerializer(), null, _serverOptions.PubSubPageSizeBytes(), true);
                broker = new SubscribeBroker<long, Block, IKeySerializer<long>>(new BlockKeyInputSerializer(), null, _serverOptions.PubSubPageSizeBytes(), true);
            }

            // Create session provider for VarLen
            provider = new BlockFasterKVProvider(_store, new BlockServerSerializer(), kvBroker, broker, _serverOptions.Recover);

            server = new FasterServerTcp(_serverOptions.Address, _serverOptions.Port);
            server.Register(WireFormat.DefaultVarLenKV, provider);
            server.Register(WireFormat.WebSocket, provider);
        }
    }
}