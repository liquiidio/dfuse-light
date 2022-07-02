using DeepReader.Storage.Faster.StoreBase;
using DeepReader.Storage.Faster.StoreBase.Client;
using DeepReader.Storage.Faster.StoreBase.Client.DeepReader.Storage.Faster.Transactions.Client;
using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.client;
using FASTER.common;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;

namespace DeepReader.Storage.Faster.Stores.ActionTraces
{
    internal class ActionTraceStoreClient : ActionTraceStoreBase
    {
        private const string Ip = "127.0.0.1";
        private const int Port = 5005;

        private readonly AsyncPool<ClientSession<ulong, ActionTrace, ActionTrace, ActionTrace, KeyValueContext,
                ClientFunctions<UlongKey, ulong, ActionTrace>, ClientSerializer<UlongKey, ulong, ActionTrace>>> _sessionPool;

        private readonly FasterKVClient<ulong, ActionTrace> _client;

        public ActionTraceStoreClient(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            _client = new FasterKVClient<ulong, ActionTrace>(Ip, Port); // TODO @Haron, IP and Port into Options/Config/appsettings.json

            _sessionPool =
                new AsyncPool<ClientSession<ulong, ActionTrace, ActionTrace, ActionTrace, KeyValueContext,
                    ClientFunctions<UlongKey, ulong, ActionTrace>, ClientSerializer<UlongKey, ulong, ActionTrace>>>(
                    size: 4,    // TODO no idea how many sessions make sense and do work,
                                // hopefully Faster-Server just blocks if it can't handle the amount of sessions and data :D
                    () => _client
                        .NewSession<ActionTrace, ActionTrace, KeyValueContext, ClientFunctions<UlongKey, ulong, ActionTrace>,
                            ClientSerializer<UlongKey, ulong, ActionTrace>>(new ClientFunctions<UlongKey, ulong, ActionTrace>(), WireFormat.WebSocket,
                            new ClientSerializer<UlongKey, ulong, ActionTrace>()));
        }

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {
            // Nothing to observe here for now, _client doesn't give any info, we probably have something to observe later
        }

        public override async Task<FASTER.core.Status> WriteActionTrace(ActionTrace actionTrace)
        {
            var actionTraceId = actionTrace.GlobalSequence;

            await EventSender.SendAsync("ActionTraceAdded", actionTrace);

            using (TypeWritingActionTraceDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                await session.UpsertAsync(actionTraceId, actionTrace);

                // UpsertAsync does not return anything here, I assume we have to SubscribeKV to manually verify something was upserted?! 
                return FASTER.core.Status.CreatePending();
            }
        }

        public override async Task<(bool, ActionTrace)> TryGetActionTraceById(ulong globalSequence)
        {
            using (TypeActionTraceReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = await session.ReadAsync(globalSequence); // No Complete()-Call here for some reason
                _sessionPool.Return(session);
                return (status.Found, output);
            }
        }
    }
}