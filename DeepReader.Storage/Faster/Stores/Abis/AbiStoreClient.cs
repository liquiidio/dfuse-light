using System.Reflection;
using DeepReader.Storage.Faster.Options;
using DeepReader.Storage.Faster.StoreBase;
using DeepReader.Storage.Faster.StoreBase.Client;
using DeepReader.Storage.Faster.StoreBase.Client.DeepReader.Storage.Faster.Transactions.Client;
using DeepReader.Storage.Faster.Stores.Abis.Custom;
using DeepReader.Types.EosTypes;
using DeepReader.Types.StorageTypes;
using FASTER.client;
using FASTER.common;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;

namespace DeepReader.Storage.Faster.Stores.Abis
{
    internal class AbiStoreClient : AbiStoreBase
    {
        private FasterClientOptions ClientOptions => (FasterClientOptions)Options;

        private readonly AsyncPool<ClientSession<ulong, AbiCacheItem, AbiCacheItem, AbiCacheItem, KeyValueContext,
                ClientAbiFunctions, ClientSerializer<UlongKey, ulong, AbiCacheItem>>>
            _sessionPool;

        private readonly FasterKVClient<ulong, AbiCacheItem> _client;

        public AbiStoreClient(FasterClientOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            _client = new FasterKVClient<ulong, AbiCacheItem>(ClientOptions.IpAddress, ClientOptions.Port);

            _sessionPool =
                new AsyncPool<ClientSession<ulong, AbiCacheItem, AbiCacheItem, AbiCacheItem, KeyValueContext,
                    ClientAbiFunctions, ClientSerializer<UlongKey, ulong, AbiCacheItem>>>(
                    size: 4,    // TODO no idea how many sessions make sense and do work,
                                // hopefully Faster-Serve just blocks if it can't handle the amount of sessions and data :D
                    () => _client
                        .NewSession<AbiCacheItem, AbiCacheItem, KeyValueContext, ClientAbiFunctions,
                            ClientSerializer<UlongKey, ulong, AbiCacheItem>>(new ClientAbiFunctions(), WireFormat.DefaultVarLenKV,
                            new ClientSerializer<UlongKey, ulong, AbiCacheItem>()));
        }

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {
            // Nothing to observe here for now, _client doesn't give any info, we probably have something to observe later
        }

        public override async Task<FASTER.core.Status> WriteAbi(AbiCacheItem abi)
        {
            var abiId = abi.Id;

            await EventSender.SendAsync("AbiAdded", abi);

            using (TypeWritingAbiDurationSummary.NewTimer())
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
            await session.RMWAsync(account.IntVal, new AbiCacheItem(account.IntVal, globalSequence, assembly));
            _sessionPool.Return(session);
        }

        public override async Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account)
        {
            using (TypeAbiReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = await session.ReadAsync(account.IntVal);
                _sessionPool.Return(session);
                return (status.Found, output);
            }
        }

        public override async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence)
        {
            using (TypeAbiReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = await session.ReadAsync(account.IntVal);
                _sessionPool.Return(session);

                if (status.Found && output.AbiVersions.Any(av => av.Key <= globalSequence))
                {
                    // returns the index of the Abi matching the globalSequence or binary complement of the next item (negative)
                    var abiVersionIndex = output.AbiVersions.Keys.ToList().BinarySearch(globalSequence);

                    // if negative, revert the binary complement
                    if (abiVersionIndex < 0)
                        abiVersionIndex = ~abiVersionIndex;
                    // we always want the previous Abi-version
                    if (abiVersionIndex > 0)
                        abiVersionIndex--;

                    var abiVersionsArray = output.AbiVersions.ToArray();
                    if (abiVersionsArray.Length > abiVersionIndex)
                        return (status.Found, abiVersionsArray[abiVersionIndex]);
                }
                return (false, new KeyValuePair<ulong, AssemblyWrapper>());
            }
        }

        public override async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account)
        {
            using (TypeAbiReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = await session.ReadAsync(account.IntVal);
                _sessionPool.Return(session);

                if (status.Found && output.AbiVersions.Count > 0)
                {
                    return (status.Found, output.AbiVersions.Last());
                }
                return (false, new KeyValuePair<ulong, AssemblyWrapper>());
            }
        }
    }
}