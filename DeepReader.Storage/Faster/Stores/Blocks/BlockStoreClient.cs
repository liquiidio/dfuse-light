using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.client;
using FASTER.common;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using System.Text;
using DeepReader.Storage.Faster.Test;
using DeepReader.Storage.Faster.Test.Client;
using DeepReader.Storage.Faster.Test.DeepReader.Storage.Faster.Transactions.Client;

namespace DeepReader.Storage.Faster.Blocks
{
    internal class BlockStoreClient : BlockStoreBase
    {
        private const string ip = "127.0.0.1";
        private const int port = 5002;
        private static Encoding _encode = Encoding.UTF8;

        private readonly AsyncPool<ClientSession<long, Block, Block, Block, KeyValueContext,
            ClientFunctions<LongKey, long, Block>, ClientSerializer<LongKey, long, Block>>> _sessionPool;

        private readonly FasterKVClient<long, Block> _client;

        public BlockStoreClient(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            _client = new FasterKVClient<long, Block>(ip, port); // TODO @Haron, IP and Port into Options/Config/appsettings.json

            _sessionPool =
                new AsyncPool<ClientSession<long, Block, Block, Block, KeyValueContext,
                    ClientFunctions<LongKey, long, Block>, ClientSerializer<LongKey, long, Block>>>(
                    size: 4,    // TODO no idea how many sessions make sense and do work,
                                // hopefully Faster-Serve just blocks if it can't handle the amount of sessions and data :D
                    () => _client
                        .NewSession<Block, Block, KeyValueContext, ClientFunctions<LongKey, long, Block>,
                            ClientSerializer<LongKey, long, Block>>(new ClientFunctions<LongKey, long, Block>(), WireFormat.WebSocket,
                            new ClientSerializer<LongKey, long, Block>()));
        }

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {

        }

        public override async Task<FASTER.core.Status> WriteBlock(Block block)
        {
            long blockId = block.Number;

            await _eventSender.SendAsync("BlockAdded", block);

            using (WritingBlockDuration.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);

                await session.UpsertAsync(blockId, block);

                return FASTER.core.Status.CreatePending();
            }
        }

        public override async Task<(bool, Block)> TryGetBlockById(uint blockNum)
        {
            using (BlockReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);

                var (status, output) = await session.ReadAsync(blockNum);
                _sessionPool.Return(session);
                return (status.Found, output);
            }
        }
    }
}