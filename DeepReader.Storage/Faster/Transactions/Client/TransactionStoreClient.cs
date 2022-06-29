using DeepReader.Storage.Faster.Transactions.Base;
using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.client;
using FASTER.common;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using System.Text;

namespace DeepReader.Storage.Faster.Transactions.Client
{
    internal class TransactionStoreClient : TransactionStoreBase
    {
        private const string ip = "127.0.0.1";
        private const int port = 5003;
        private readonly AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace, TransactionContext, TransactionClientFunctions, TransactionClientSerializer>> _sessionPool;

        private readonly FasterKVClient<TransactionId, TransactionTrace> _client;

        public TransactionStoreClient(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            _client = new FasterKVClient<TransactionId, TransactionTrace>(ip, port); // TODO @Haron, IP and Port into Options/Config/appsettings.json

            _sessionPool =
                new AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace, TransactionContext,
                    TransactionClientFunctions, TransactionClientSerializer>>(
                    size: 4,    // TODO no idea how many sessions make sense and do work,
                                // hopefully Faster-Serve just blocks if it can't handle the amount of sessions and data :D
                    () => _client
                        .NewSession<TransactionTrace, TransactionTrace, TransactionContext, TransactionClientFunctions,
                            TransactionClientSerializer>(new TransactionClientFunctions(), WireFormat.DefaultVarLenKV,
                            new TransactionClientSerializer()));

        }

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {

        }

        public override async Task<FASTER.core.Status> WriteTransaction(TransactionTrace transaction)
        {
            var transactionId = new TransactionId(transaction.Id);

            await _eventSender.SendAsync("TransactionAdded", transaction);

            using (WritingTransactionDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                await session.UpsertAsync(transactionId, transaction);
                return FASTER.core.Status.CreatePending();
            }
        }

        public override async Task<(bool, TransactionTrace)> TryGetTransactionTraceById(Types.Eosio.Chain.TransactionId transactionId)
        {
            using (TransactionReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = await session.ReadAsync(new TransactionId(transactionId));
                _sessionPool.Return(session);
                return (status.Found, output);
            }
        }
    }
}