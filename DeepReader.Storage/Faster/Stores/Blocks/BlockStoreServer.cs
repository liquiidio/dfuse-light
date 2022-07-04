using DeepReader.Storage.Faster.Options;
using DeepReader.Storage.Faster.StoreBase.Server;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;
using FASTER.server;
using HotChocolate.Subscriptions;

namespace DeepReader.Storage.Faster.Stores.Blocks
{
    public class BlockStoreServer : BlockStore
    {
        private FasterServerOptions ServerOptions => (FasterServerOptions)Options;

        readonly IFasterServer _server;
        readonly ServerKVProvider<LongKey, long, Block> _provider;
        readonly SubscribeKVBroker<long, Block, Block, IKeyInputSerializer<long, Block>> _kvBroker;
        readonly SubscribeBroker<long, Block, IKeySerializer<long>> _broker;

        public BlockStoreServer(IFasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            if (!ServerOptions.DisablePubSub)
            {
                _kvBroker = new SubscribeKVBroker<long, Block, Block, IKeyInputSerializer<long, Block>>(
                    new ServerKeyInputSerializer<LongKey, long, Block>(), null, ServerOptions.PubSubPageSizeBytes, true);
                _broker = new SubscribeBroker<long, Block, IKeySerializer<long>>(
                    new ServerKeyInputSerializer<LongKey, long, Block>(), null, ServerOptions.PubSubPageSizeBytes, true);
            }

            // Create session provider for VarLen
            _provider = new ServerKVProvider<LongKey, long, Block>(Store, new ServerSerializer<LongKey, long, Block>(), _kvBroker, _broker);

            _server = new FasterServerTcp(ServerOptions.IpAddress, ServerOptions.Port);
            _server.Register(WireFormat.DefaultVarLenKV, _provider);
            _server.Register(WireFormat.WebSocket, _provider);
        }
    }
}