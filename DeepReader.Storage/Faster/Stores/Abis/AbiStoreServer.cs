using DeepReader.Storage.Faster.Options;
using DeepReader.Storage.Faster.StoreBase.Server;
using DeepReader.Storage.Faster.Stores.Abis.Custom;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.server;
using HotChocolate.Subscriptions;

namespace DeepReader.Storage.Faster.Stores.Abis
{
    internal class AbiStoreServer : AbiStore
    {
        private FasterServerOptions ServerOptions => (FasterServerOptions)Options;

        readonly IFasterServer _server;
        readonly ServerAbiKVProvider _provider;
        readonly SubscribeKVBroker<ulong, AbiCacheItem, AbiCacheItem, IKeyInputSerializer<ulong, AbiCacheItem>> _kvBroker;
        readonly SubscribeBroker<ulong, AbiCacheItem, IKeySerializer<ulong>> _broker;

        public AbiStoreServer(IFasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            if (!ServerOptions.DisablePubSub)
            {
                _kvBroker = new SubscribeKVBroker<ulong, AbiCacheItem, AbiCacheItem, IKeyInputSerializer<ulong, AbiCacheItem>>(
                    new ServerKeyInputSerializer<UlongKey, ulong, AbiCacheItem>(), null, ServerOptions.PubSubPageSizeBytes, true);
                _broker = new SubscribeBroker<ulong, AbiCacheItem, IKeySerializer<ulong>>(
                    new ServerKeyInputSerializer<UlongKey, ulong, AbiCacheItem>(), null, ServerOptions.PubSubPageSizeBytes, true);
            }

            // Create session provider for VarLen
            _provider = new ServerAbiKVProvider(Store, new ServerSerializer<UlongKey, ulong, AbiCacheItem>(), _kvBroker, _broker);

            _server = new FasterServerTcp(ServerOptions.IpAddress, ServerOptions.AbiStorePort);
            _server.Register(WireFormat.DefaultVarLenKV, _provider);
            _server.Register(WireFormat.WebSocket, _provider);
        }
    }
}