using DeepReader.Types.StorageTypes;
using FASTER.client;
using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using DeepReader.Storage.Options;
using FASTER.common;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Status = FASTER.core.Status;
using DeepReader.Storage.Faster.ActionTraces.Base;
using DeepReader.Storage.Faster.ActionTraces.Standalone;

namespace DeepReader.Storage.Faster.ActionTraces.Client
{
    internal class ActionTraceStoreClient : ActionTraceStoreBase
    {
        private const string ip = "127.0.0.1";
        private const int port = 5005;
        private static Encoding _encode = Encoding.UTF8;
        private readonly AsyncPool<ClientSession<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceContext, ActionTraceClientFunctions, ActionTraceClientSerializer>> _sessionPool;

        private readonly FasterKVClient<ulong, ActionTrace> _client;

        public ActionTraceStoreClient(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            _client = new FasterKVClient<ulong, ActionTrace>(ip, port); // TODO @Haron, IP and Port into Options/Config/appsettings.json

            _sessionPool =
                new AsyncPool<ClientSession<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, ActionTraceContext,
                    ActionTraceClientFunctions, ActionTraceClientSerializer>>(
                    size: 4,    // TODO no idea how many sessions make sense and do work,
                                // hopefully Faster-Serve just blocks if it can't handle the amount of sessions and data :D
                    () => _client
                        .NewSession<ActionTraceInput, ActionTraceOutput, ActionTraceContext, ActionTraceClientFunctions,
                            ActionTraceClientSerializer>(new ActionTraceClientFunctions(), WireFormat.WebSocket,
                            new ActionTraceClientSerializer()));
        }

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {
            // Nothing to observe here for now, _client doesn't give any info, we probably have something to observe later
        }

        public override async Task<Status> WriteActionTrace(ActionTrace actionTrace)
        {
            var actionTraceId = actionTrace.GlobalSequence;

            await _eventSender.SendAsync("ActionTraceAdded", actionTrace);

            using (WritingActionTraceDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                await session.UpsertAsync(actionTraceId, actionTrace);

                // UpsertAsync does not return anything here, I assume we have to SubscribeKV to manually verify something was upserted?! 
                return Status.CreatePending();
            }
        }

        public override async Task<(bool, ActionTrace)> TryGetActionTraceById(ulong globalSequence)
        {
            using (ActionTraceReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = await session.ReadAsync(globalSequence); // No Complete()-Call here for some reason
                _sessionPool.Return(session);
                return (status.Found, output.Value);
            }
        }
    }
}