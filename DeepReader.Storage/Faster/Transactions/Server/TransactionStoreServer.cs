﻿using DeepReader.Storage.Faster.Test.Server;
using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Storage.Options;
using DeepReader.Types.Eosio.Chain;
using FASTER.common;
using FASTER.core;
using FASTER.server;
using HotChocolate.Subscriptions;
using TransactionTrace = DeepReader.Types.StorageTypes.TransactionTrace;

namespace DeepReader.Storage.Faster.Transactions.Server
{
    public class TransactionStoreServer : TransactionStore
    {
        readonly ServerOptions _serverOptions;
        readonly IFasterServer server;
        readonly ServerKVProvider<TransactionId, TransactionId, TransactionTrace> provider;
        readonly SubscribeKVBroker<TransactionId, TransactionTrace, TransactionId, IKeyInputSerializer<TransactionId,TransactionId>> kvBroker;
        readonly SubscribeBroker<TransactionId, TransactionTrace, IKeySerializer<TransactionId>> broker;
        readonly LogSettings logSettings;


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
                kvBroker =
                    new SubscribeKVBroker<TransactionId, TransactionTrace, TransactionId,
                        IKeyInputSerializer<TransactionId, TransactionId>>(new ServerKeyInputSerializer<TransactionId, TransactionId>(), null,
                        _serverOptions.PubSubPageSizeBytes(), true);
                broker = new SubscribeBroker<TransactionId, TransactionTrace, IKeySerializer<TransactionId>>(
                        new ServerKeyInputSerializer<TransactionId, TransactionId>(), null, 
                        _serverOptions.PubSubPageSizeBytes(), true);
            }

            provider = new ServerKVProvider<TransactionId, TransactionId, TransactionTrace>(_store, new ServerSerializer<TransactionId, TransactionId, TransactionTrace>(), kvBroker, broker, _serverOptions.Recover);

            server = new FasterServerTcp(_serverOptions.Address, _serverOptions.Port);
            server.Register(WireFormat.DefaultVarLenKV, provider);
            server.Register(WireFormat.WebSocket, provider);

            server.Start();
        }
    }
}