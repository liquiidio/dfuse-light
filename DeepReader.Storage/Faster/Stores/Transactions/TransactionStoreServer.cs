using DeepReader.Storage.Faster.Options;
using DeepReader.Storage.Faster.StoreBase.Server;
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
        private FasterServerOptions ServerOptions => (FasterServerOptions)Options;

        readonly IFasterServer _server;
        readonly ServerKVProvider<TransactionId, TransactionId, TransactionTrace> _provider;
        readonly SubscribeKVBroker<TransactionId, TransactionTrace, TransactionTrace, IKeyInputSerializer<TransactionId, TransactionTrace>> _kvBroker;
        readonly SubscribeBroker<TransactionId, TransactionTrace, IKeySerializer<TransactionId>> _broker;

        public TransactionStoreServer(FasterStandaloneOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            if (!ServerOptions.DisablePubSub)
            {
                _kvBroker =
                    new SubscribeKVBroker<TransactionId, TransactionTrace, TransactionTrace,
                        IKeyInputSerializer<TransactionId, TransactionTrace>>(new ServerKeyInputSerializer<TransactionId, TransactionId, TransactionTrace>(), null,
                        ServerOptions.PubSubPageSizeBytes, true);
                _broker = new SubscribeBroker<TransactionId, TransactionTrace, IKeySerializer<TransactionId>>(
                        new ServerKeyInputSerializer<TransactionId, TransactionId, TransactionId>(), null,
                        ServerOptions.PubSubPageSizeBytes, true);
            }

            _provider = new ServerKVProvider<TransactionId, TransactionId, TransactionTrace>(Store, new ServerSerializer<TransactionId, TransactionId, TransactionTrace>(), _kvBroker, _broker);

            _server = new FasterServerTcp(ServerOptions.IpAddress, ServerOptions.Port);
            _server.Register(WireFormat.DefaultVarLenKV, _provider);
            _server.Register(WireFormat.WebSocket, _provider);

            _server.Start();
        }
    }
}