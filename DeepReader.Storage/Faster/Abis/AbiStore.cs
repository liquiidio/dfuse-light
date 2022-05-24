using DeepReader.Storage.Options;
using DeepReader.Types.EosTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Sentry;
using Serilog;
using System.Reflection;

namespace DeepReader.Storage.Faster.Abis
{
    public class AbiStore
    {
        private readonly FasterKV<AbiId, AbiCacheItem> _store;

        private readonly ClientSession<AbiId, AbiCacheItem, AbiInput, AbiOutput, AbiContext, AbiFunctions> _AbiReaderSession;
        private readonly ClientSession<AbiId, AbiCacheItem, AbiInput, AbiOutput, AbiContext, AbiFunctions> _AbiWriterSession;

        private FasterStorageOptions _options;

        private ITopicEventSender _eventSender;

        private static readonly Histogram _writingAbiDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_write_abi_duration", "Histogram of time to store Abis to Faster");
        private static readonly Histogram _storeLogMemorySizeBytesHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_abi_store_log_memory_size_bytes", "Histogram of the faster Abi store log memory size in bytes");
        private static readonly Histogram _storeReadCacheMemorySizeBytesHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_abi_store_read_cache_memory_size_bytes", "Histogram of the faster Abi store read cache memory size in bytes");
        private static readonly Histogram _storeEntryCountHistogram =
           Metrics.CreateHistogram("deepreader_storage_faster_abi_store_read_cache_memory_size_bytes", "Histogram of the faster Abi store entry count");
        private static readonly Histogram _storeTakeFullCheckpointDurationHistogram =
          Metrics.CreateHistogram("deepreader_storage_faster_abi_store_take_full_checkpoint_duration", "Histogram of time to take a full checkpoint of faster Abi store");
        private static readonly Histogram _storeFlushAndEvictLogDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_abi_store_log_flush_and_evict_duration", "Histogram of time to flush and evict faster Abi store");
        private static readonly Histogram _AbiReaderSessionReadDurationHistogram =
          Metrics.CreateHistogram("deepreader_storage_faster_abi_get_by_id_duration", "Histogram of time to try get Abi trace by id");

        public AbiStore(FasterStorageOptions options, ITopicEventSender eventSender)
        {
            _options = options;
            _eventSender = eventSender;

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
                PageSizeBits = 12, // (4K pages)
                MemorySizeBits = 34 // (16G memory for main log)
            };

            // Define serializers; otherwise FASTER will use the slower DataContract
            // Needed only for class keys/values
            var serializerSettings = new SerializerSettings<AbiId, AbiCacheItem>
            {
                keySerializer = () => new AbiIdSerializer(),
                valueSerializer = () => new AbiValueSerializer()
            };

            var checkPointsDir = _options.AbiStoreDir + "checkpoints";

            var checkpointManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(checkPointsDir), true);

            _store = new FasterKV<AbiId, AbiCacheItem>(
                size: _options.MaxAbiCacheEntries, // Cache Lines for Abis
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointManager = checkpointManager },
                serializerSettings: serializerSettings,
                comparer: new AbiId(0)
            );

            if (Directory.Exists(checkPointsDir))
            {
                Log.Information("Recovering AbiStore");
                _store.Recover(1);
                Log.Information("AbiStore recovered");
            }

            foreach (var recoverableSession in _store.RecoverableSessions)
            {
                if (recoverableSession.Item2 == "AbiWriterSession")
                {
                    _AbiWriterSession = _store.For(new AbiFunctions())
                        .ResumeSession<AbiFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
                }
                else if (recoverableSession.Item2 == "AbiReaderSession")
                {
                    _AbiReaderSession = _store.For(new AbiFunctions())
                        .ResumeSession<AbiFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
                }
            }

            _AbiWriterSession ??=
                _store.For(new AbiFunctions()).NewSession<AbiFunctions>("AbiWriterSession");
            _AbiReaderSession ??=
                _store.For(new AbiFunctions()).NewSession<AbiFunctions>("AbiReaderSession");

            _storeLogMemorySizeBytesHistogram.Observe(_store.Log.MemorySizeBytes);
            if (options.UseReadCache)
                _storeReadCacheMemorySizeBytesHistogram.Observe(_store.ReadCache.MemorySizeBytes);
            _storeEntryCountHistogram.Observe(_store.EntryCount);

            // TODO, for some reason I need to manually call the Init
            SentrySdk.Init("https://b4874920c4484212bcc323e9deead2e9@sentry.noodles.lol/2");

            new Thread(CommitThread).Start();
        }

        public async Task<Status> WriteAbi(AbiCacheItem Abi)
        {
            var AbiId = new AbiId(Abi.Id);

            await _eventSender.SendAsync("AbiAdded", Abi);

            using (_writingAbiDurationHistogram.NewTimer())
            {
                var result = await _AbiWriterSession.UpsertAsync(ref AbiId, ref Abi);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                return result.Status;
            }
        }

        public async Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly)
        {
            (await _AbiWriterSession.RMWAsync(new AbiId(account.IntVal), new AbiInput(account.IntVal, globalSequence, assembly), new AbiContext())).Complete();
        }

        public async Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account)
        {
            using (_AbiReaderSessionReadDurationHistogram.NewTimer())
            {
                var (status, output) = (await _AbiReaderSession.ReadAsync(new AbiId(account.IntVal))).Complete();
                return (status.Found, output.Value);
            }
        }

        public async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence)
        {
            using (_AbiReaderSessionReadDurationHistogram.NewTimer())
            {
                var (status, output) = (await _AbiReaderSession.ReadAsync(new AbiId(account.IntVal))).Complete();

                if(status.Found && output.Value.AbiVersions.Any(av => av.Key <= globalSequence))
                {
                    // returns the index of the Abi matching the globalSequence or binary complement of the next item (negative)
                    var abiVersionIndex = output.Value.AbiVersions.Keys.ToList().BinarySearch(globalSequence);

                    // if negative, revert the binary complement
                    if (abiVersionIndex < 0)
                        abiVersionIndex = ~abiVersionIndex;
                    // we always want the previous Abi-version
                    if(abiVersionIndex > 0)
                        abiVersionIndex--;

                    var abiVersionsArry = output.Value.AbiVersions.ToArray();
                    if (abiVersionIndex >= 0 && abiVersionsArry.Length > abiVersionIndex)
                        return (status.Found, abiVersionsArry[abiVersionIndex]);
                }
                return (false, new KeyValuePair<ulong, AssemblyWrapper>());
            }
        }

        public async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account)
        {
            using (_AbiReaderSessionReadDurationHistogram.NewTimer())
            {
                var (status, output) = (await _AbiReaderSession.ReadAsync(new AbiId(account.IntVal))).Complete();

                if (status.Found && output.Value.AbiVersions.Count > 0)
                {
                    return (status.Found, output.Value.AbiVersions.Last());
                }
                return (false, new KeyValuePair<ulong, AssemblyWrapper>());
            }
        }

        private void CommitThread()
        {
            if (_options.CheckpointInterval is null or 0)
                return;

            while (true)
            {
                try
                {
                    Thread.Sleep(_options.CheckpointInterval.Value);

                    // Take log-only checkpoint (quick - no index save)
                    //store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver).GetAwaiter().GetResult();

                    // Take index + log checkpoint (longer time)
                    using (_storeTakeFullCheckpointDurationHistogram.NewTimer())
                        _store.TakeFullCheckpointAsync(CheckpointType.FoldOver).GetAwaiter().GetResult();
                    using (_storeFlushAndEvictLogDurationHistogram.NewTimer())
                        _store.Log.FlushAndEvict(true);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "");
                }
            }
        }
    }
}
