﻿using DeepReader.Storage.Faster.Options;
using DeepReader.Storage.Faster.StoreBase;
using DeepReader.Storage.Faster.StoreBase.Client;
using DeepReader.Storage.Faster.StoreBase.Client.DeepReader.Storage.Faster.Transactions.Client;
using DeepReader.Types.Eosio.Chain;
using FASTER.client;
using FASTER.common;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using TransactionTrace = DeepReader.Types.StorageTypes.TransactionTrace;

namespace DeepReader.Storage.Faster.Stores.Transactions
{
    internal class TransactionStoreClient : TransactionStoreBase
    {
        private FasterClientOptions ClientOptions => (FasterClientOptions)Options;

        private readonly AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace,
                KeyValueContext, ClientFunctions<TransactionId, TransactionId, TransactionTrace>,
                ClientSerializer<TransactionId, TransactionId, TransactionTrace>>>
            _sessionPool;

        private readonly
            AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace,
                KeyValueContext, ClientFunctions<TransactionId, TransactionId, TransactionTrace>,
                ClientSerializer<TransactionId, TransactionId, TransactionTrace>>>
            _readerSessionPool;

        private readonly FasterKVClient<TransactionId, TransactionTrace> _client;

        public TransactionStoreClient(IFasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            _client = new FasterKVClient<TransactionId, TransactionTrace>(ClientOptions.IpAddress, ClientOptions.Port);

            _sessionPool =
                new AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace, KeyValueContext,
                    ClientFunctions<TransactionId, TransactionId, TransactionTrace>, ClientSerializer<TransactionId, TransactionId, TransactionTrace>>>(
                    size: 20,    // TODO no idea how many sessions make sense and do work,
                                 // hopefully Faster-Server just blocks if it can't handle the amount of sessions and data :D
                    () => _client
                        .NewSession<TransactionTrace, TransactionTrace, KeyValueContext, ClientFunctions<TransactionId, TransactionId, TransactionTrace>,
                            ClientSerializer<TransactionId, TransactionId, TransactionTrace>>(new ClientFunctions<TransactionId, TransactionId, TransactionTrace>(), WireFormat.DefaultVarLenKV,
                            new ClientSerializer<TransactionId, TransactionId, TransactionTrace>()));

            _readerSessionPool =
                new AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace, KeyValueContext,
                    ClientFunctions<TransactionId, TransactionId, TransactionTrace>, ClientSerializer<TransactionId, TransactionId, TransactionTrace>>>(
                    size: 20,    // TODO no idea how many sessions make sense and do work,
                                 // hopefully Faster-Server just blocks if it can't handle the amount of sessions and data :D
                    () => _client
                        .NewSession<TransactionTrace, TransactionTrace, KeyValueContext, ClientFunctions<TransactionId, TransactionId, TransactionTrace>,
                            ClientSerializer<TransactionId, TransactionId, TransactionTrace>>(new ClientFunctions<TransactionId, TransactionId, TransactionTrace>(), WireFormat.DefaultVarLenKV,
                            new ClientSerializer<TransactionId, TransactionId, TransactionTrace>()));

        }

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {

        }

        public override async Task<FASTER.core.Status> WriteTransaction(TransactionTrace transaction)
        {
            var transactionId = new TransactionId(transaction.Id);

            await EventSender.SendAsync("TransactionAdded", transaction);

            using (TypeWritingTransactionDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                await session.UpsertAsync(transactionId, transaction);
                _sessionPool.Return(session);
                return FASTER.core.Status.CreatePending();
            }
        }

        public override async Task<(bool, TransactionTrace)> TryGetTransactionTraceById(TransactionId transactionId)
        {
            using (TypeTransactionReaderSessionReadDurationSummary.NewTimer())
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