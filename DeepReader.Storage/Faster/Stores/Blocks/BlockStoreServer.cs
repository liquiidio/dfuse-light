using DeepReader.Storage.Faster.StoreBase.Server;
using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;
using FASTER.server;
using HotChocolate.Subscriptions;

namespace DeepReader.Storage.Faster.Stores.Blocks
{
    public class BlockStoreServer : BlockStore
    {
        readonly ServerOptions _serverOptions;
        readonly IFasterServer _server;
        readonly ServerKvProvider<LongKey, long, Block> _provider;
        readonly SubscribeKVBroker<long, Block, long, IKeyInputSerializer<long, long>> _kvBroker;
        readonly SubscribeBroker<long, Block, IKeySerializer<long>> _broker;
        readonly LogSettings _logSettings;


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
                _kvBroker = new SubscribeKVBroker<long, Block, long, IKeyInputSerializer<long, long>>(
                    new ServerKeyInputSerializer<LongKey, long>(), null, _serverOptions.PubSubPageSizeBytes(), true);
                _broker = new SubscribeBroker<long, Block, IKeySerializer<long>>(
                    new ServerKeyInputSerializer<LongKey, long>(), null, _serverOptions.PubSubPageSizeBytes(), true);
            }

            // Create session provider for VarLen
            _provider = new ServerKvProvider<LongKey, long, Block>(Store, new ServerSerializer<LongKey, long, Block>(), _kvBroker, _broker, _serverOptions.Recover);

            _server = new FasterServerTcp(_serverOptions.Address, _serverOptions.Port);
            _server.Register(WireFormat.DefaultVarLenKV, _provider);
            _server.Register(WireFormat.WebSocket, _provider);
        }
    }
}