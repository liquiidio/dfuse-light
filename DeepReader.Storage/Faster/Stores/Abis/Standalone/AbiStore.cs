using DeepReader.Storage.Faster.Abis.Base;
using DeepReader.Storage.Options;
using DeepReader.Types.EosTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Serilog;
using System.Reflection;

namespace DeepReader.Storage.Faster.Abis.Standalone
{
    public class AbiStore : AbiStoreBase
    {
        protected readonly FasterKV<ulong, AbiCacheItem> _store;

        private readonly AsyncPool<ClientSession<ulong, AbiCacheItem, AbiInput, AbiOutput, AbiContext, AbiFunctions>> _sessionPool;

        public AbiStore(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector) : base(options, eventSender, metricsCollector)
        {
            if (!_options.AbiStoreDir.EndsWith("/"))
                options.AbiStoreDir += "/";

            // Create files for storing data
            var log = Devices.CreateLogDevice(_options.AbiStoreDir + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(_options.AbiStoreDir + "hlog.obj.log");

            // Define settings for log
            var logSettings = new LogSettings
            {
                LogDevice = log,
                ObjectLogDevice = objlog,
                ReadCacheSettings = _options.UseReadCache ? new ReadCacheSettings() : null,
                // to calculate below:
                // 12 = 00001111 11111111 = 4095 = 4K
                // 34 = 00000011 11111111 11111111 11111111 11111111 = 17179869183 = 16G
                PageSizeBits = 14, // (4K pages)
                MemorySizeBits = 28 // (250M memory for main log)
            };

            // Define serializers; otherwise FASTER will use the slower DataContract
            // Needed only for class keys/values
            var serializerSettings = new SerializerSettings<ulong, AbiCacheItem>
            {
                valueSerializer = () => new AbiValueSerializer()
            };

            var checkPointsDir = _options.AbiStoreDir + "checkpoints";

            var checkpointManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(checkPointsDir), true);

            _store = new FasterKV<ulong, AbiCacheItem>(
                size: _options.MaxAbiCacheEntries, // Cache Lines for Abis
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointManager = checkpointManager },
                serializerSettings: serializerSettings,
                new FasterSequentialULongKeyComparer()
            );

            if (Directory.Exists(checkPointsDir))
            {
                try
                {
                    Log.Information("Recovering AbiStore");
                    _store.Recover(1);
                    Log.Information("AbiStore recovered");
                }
                catch (Exception e)
                {
                    Log.Error(e, "");
                    throw;
                }
            }

            //_abiWriterSession ??=
            //    _store.For(new AbiFunctions()).NewSession<AbiFunctions>("AbiWriterSession");
            //_abiReaderSession ??=
            //    _store.For(new AbiFunctions()).NewSession<AbiFunctions>("AbiReaderSession");

            _sessionPool = new AsyncPool<ClientSession<ulong, AbiCacheItem, AbiInput, AbiOutput, AbiContext, AbiFunctions>>(
                logSettings.LogDevice.ThrottleLimit,
                () => _store.For(new AbiFunctions()).NewSession<AbiFunctions>("AbiSession" + Interlocked.Increment(ref _sessionCount)));

            foreach (var recoverableSession in _store.RecoverableSessions)
            {
                _sessionPool.Return(_store.For(new AbiFunctions())
                    .ResumeSession<AbiFunctions>(recoverableSession.Item2, out CommitPoint commitPoint));
                _sessionCount++;
            }

            new Thread(CommitThread).Start();
        }

        protected override void CollectObservableMetrics(object? sender, EventArgs e)
        {
            StoreLogMemorySizeBytesSummary.Observe(_store.Log.MemorySizeBytes);
            if (_options.UseReadCache)
                StoreReadCacheMemorySizeBytesSummary.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountSummary.Observe(_store.EntryCount);
        }

        public override async Task<Status> WriteAbi(AbiCacheItem abi)
        {
            var abiId = abi.Id;

            await _eventSender.SendAsync("AbiAdded", abi);

            using (WritingAbiDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var result = await session.UpsertAsync(ref abiId, ref abi);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                _sessionPool.Return(session);
                return result.Status;
            }
        }

        public override async Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly)
        {
            if (!_sessionPool.TryGet(out var session))
                session = await _sessionPool.GetAsync().ConfigureAwait(false);
            (await session.RMWAsync(account.IntVal, new AbiInput(account.IntVal, globalSequence, assembly))).Complete();
            _sessionPool.Return(session);
        }

        public override async Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account)
        {
            using (AbiReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = (await session.ReadAsync(account.IntVal)).Complete();
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
                var (status, output) = (await session.ReadAsync(account.IntVal)).Complete();
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
                var (status, output) = (await session.ReadAsync(account.IntVal)).Complete();
                _sessionPool.Return(session);

                if (status.Found && output.Value.AbiVersions.Count > 0)
                {
                    return (status.Found, output.Value.AbiVersions.Last());
                }
                return (false, new KeyValuePair<ulong, AssemblyWrapper>());
            }
        }

        private void CommitThread()
        {
            if (_options.LogCheckpointInterval is null or 0)
                return;

            int logCheckpointsTaken = 0;

            while (true)
            {
                try
                {
                    Thread.Sleep(_options.LogCheckpointInterval.Value);

                    using (StoreTakeLogCheckpointDurationSummary.NewTimer())
                        _store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver, true).GetAwaiter().GetResult();

                    if (_options.FlushAfterCheckpoint)
                    {
                        using (StoreFlushAndEvictLogDurationSummary.NewTimer())
                            _store.Log.FlushAndEvict(true);
                    }

                    if (logCheckpointsTaken % _options.IndexCheckpointMultiplier == 0)
                    {
                        using (StoreTakeIndexCheckpointDurationSummary.NewTimer())
                            _store.TakeIndexCheckpointAsync().GetAwaiter().GetResult();
                    }

                    logCheckpointsTaken++;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "");
                }
            }
        }
    }
}
