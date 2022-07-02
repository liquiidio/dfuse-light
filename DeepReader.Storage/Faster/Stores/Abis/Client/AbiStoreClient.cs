using System.Reflection;
using System.Text;
using DeepReader.Storage.Faster.Stores.Abis.Base;
using DeepReader.Storage.Faster.Stores.Abis.Standalone;
using DeepReader.Storage.Options;
using DeepReader.Types.EosTypes;
using FASTER.client;
using FASTER.common;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;

namespace DeepReader.Storage.Faster.Stores.Abis.Client
{
    internal class AbiStoreClient : AbiStoreBase
    {
        private const string ip = "127.0.0.1";
        private const int port = 5004;
        private static Encoding _encode = Encoding.UTF8;
        private readonly AsyncPool<ClientSession<ulong, AbiCacheItem, AbiInput, AbiOutput, AbiContext, AbiClientFunctions, AbiClientSerializer>> _sessionPool;

        private readonly FasterKVClient<ulong, AbiCacheItem> _client;

        public AbiStoreClient(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            _client = new FasterKVClient<ulong, AbiCacheItem>(ip, port); // TODO @Haron, IP and Port into Options/Config/appsettings.json

            _sessionPool =
                new AsyncPool<ClientSession<ulong, AbiCacheItem, AbiInput, AbiOutput, AbiContext,
                    AbiClientFunctions, AbiClientSerializer>>(
                    size: 4,    // TODO no idea how many sessions make sense and do work,
                                // hopefully Faster-Serve just blocks if it can't handle the amount of sessions and data :D
                    () => _client
                        .NewSession<AbiInput, AbiOutput, AbiContext, AbiClientFunctions,
                            AbiClientSerializer>(new AbiClientFunctions(), WireFormat.WebSocket,
                            new AbiClientSerializer()));
        }

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {
            // Nothing to observe here for now, _client doesn't give any info, we probably have something to observe later
        }

        public override async Task<FASTER.core.Status> WriteAbi(AbiCacheItem abi)
        {
            var abiId = abi.Id;

            await _eventSender.SendAsync("AbiAdded", abi);

            using (WritingAbiDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                await session.UpsertAsync(abiId, abi);
                return FASTER.core.Status.CreatePending();
            }
        }

        public override async Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly)
        {
            if (!_sessionPool.TryGet(out var session))
                session = await _sessionPool.GetAsync().ConfigureAwait(false);
            await session.RMWAsync(account.IntVal, new AbiInput(account.IntVal, globalSequence, assembly));
            _sessionPool.Return(session);
        }

        public override async Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account)
        {
            using (AbiReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = await session.ReadAsync(account.IntVal);
                _sessionPool.Return(session);
                return (status.Found, output.Value);
            }
        }

        public override async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence)
        {
            using (AbiReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = await session.ReadAsync(account.IntVal);
                _sessionPool.Return(session);

                if (status.Found && output.Value.AbiVersions.Any(av => av.Key <= globalSequence))
                {
                    // returns the index of the Abi matching the globalSequence or binary complement of the next item (negative)
                    var abiVersionIndex = output.Value.AbiVersions.Keys.ToList().BinarySearch(globalSequence);

                    // if negative, revert the binary complement
                    if (abiVersionIndex < 0)
                        abiVersionIndex = ~abiVersionIndex;
                    // we always want the previous Abi-version
                    if (abiVersionIndex > 0)
                        abiVersionIndex--;

                    var abiVersionsArry = output.Value.AbiVersions.ToArray();
                    if (abiVersionIndex >= 0 && abiVersionsArry.Length > abiVersionIndex)
                        return (status.Found, abiVersionsArry[abiVersionIndex]);
                }
                return (false, new KeyValuePair<ulong, AssemblyWrapper>());
            }
        }

        public override async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account)
        {
            using (AbiReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = await session.ReadAsync(account.IntVal);
                _sessionPool.Return(session);

                if (status.Found && output.Value.AbiVersions.Count > 0)
                {
                    return (status.Found, output.Value.AbiVersions.Last());
                }
                return (false, new KeyValuePair<ulong, AssemblyWrapper>());
            }
        }
    }
}