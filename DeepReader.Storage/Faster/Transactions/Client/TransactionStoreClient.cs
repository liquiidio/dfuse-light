﻿using DeepReader.Storage.Faster.Transactions.Base;
using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Storage.Options;
using FASTER.client;
using FASTER.common;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using System.Text;
using DeepReader.Storage.Faster.Test.DeepReader.Storage.Faster.Transactions.Client;
using DeepReader.Types.Eosio.Chain;
using TransactionTrace = DeepReader.Types.StorageTypes.TransactionTrace;
using DeepReader.Storage.Faster.Test.Client;
using DeepReader.Storage.Faster.Test;

namespace DeepReader.Storage.Faster.Transactions.Client
{
    internal class TransactionStoreClient : TransactionStoreBase
    {
        private const string ip = "127.0.0.1";
        private const int port = 5003;

        private readonly AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace,
                KeyValueContext, KeyValueClientFunctions<TransactionId, TransactionTrace>,
                KeyValueClientSerializer<TransactionId, TransactionTrace>>>
            _sessionPool;

        private readonly
            AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace,
                KeyValueContext, KeyValueClientFunctions<TransactionId, TransactionTrace>,
                KeyValueClientSerializer<TransactionId, TransactionTrace>>> 
            _readerSessionPool;

        private readonly FasterKVClient<TransactionId, TransactionTrace> _client;

        public TransactionStoreClient(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            _client = new FasterKVClient<TransactionId, TransactionTrace>(ip, port); // TODO @Haron, IP and Port into Options/Config/appsettings.json

            _sessionPool =
                new AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace, KeyValueContext,
                    KeyValueClientFunctions<TransactionId, TransactionTrace>, KeyValueClientSerializer<TransactionId, TransactionTrace>>>(
                    size: 20,    // TODO no idea how many sessions make sense and do work,
                                // hopefully Faster-Serve just blocks if it can't handle the amount of sessions and data :D
                    () => _client
                        .NewSession<TransactionTrace, TransactionTrace, KeyValueContext, KeyValueClientFunctions<TransactionId, TransactionTrace>,
                            KeyValueClientSerializer<TransactionId, TransactionTrace>>(new KeyValueClientFunctions<TransactionId, TransactionTrace>(), WireFormat.DefaultVarLenKV,
                            new KeyValueClientSerializer<TransactionId, TransactionTrace>()));

            _readerSessionPool =
                new AsyncPool<ClientSession<TransactionId, TransactionTrace, TransactionTrace, TransactionTrace, KeyValueContext,
                    KeyValueClientFunctions<TransactionId, TransactionTrace>, KeyValueClientSerializer<TransactionId, TransactionTrace>>>(
                    size: 20,    // TODO no idea how many sessions make sense and do work,
                    // hopefully Faster-Serve just blocks if it can't handle the amount of sessions and data :D
                    () => _client
                        .NewSession<TransactionTrace, TransactionTrace, KeyValueContext, KeyValueClientFunctions<TransactionId, TransactionTrace>,
                            KeyValueClientSerializer<TransactionId, TransactionTrace>>(new KeyValueClientFunctions<TransactionId, TransactionTrace>(), WireFormat.DefaultVarLenKV,
                            new KeyValueClientSerializer<TransactionId, TransactionTrace>()));

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
                _sessionPool.Return(session);
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