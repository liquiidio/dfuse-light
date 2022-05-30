using DeepReader.Storage.Options;
using DeepReader.Types.StorageTypes;
using FASTER.core;
using HotChocolate.Subscriptions;
using Prometheus;
using Serilog;

namespace DeepReader.Storage.Faster.Blocks
{
    public sealed class BlockStore
    {
        private readonly FasterKV<long, Block> _store;

        //private readonly ClientSession<long, Block, BlockInput, BlockOutput, BlockContext, BlockFunctions> _blockWriterSession;
        //private readonly ClientSession<long, Block, BlockInput, BlockOutput, BlockContext, BlockFunctions> _blockReaderSession;

        private readonly AsyncPool<ClientSession<long, Block, BlockInput, BlockOutput, BlockContext, BlockFunctions>> _sessionPool;

        private int _sessionCount;

        private readonly FasterStorageOptions _options;


        private ITopicEventSender _eventSender;
        private MetricsCollector _metricsCollector;

        private static readonly SummaryConfiguration SummaryConfiguration = new SummaryConfiguration()
            {MaxAge = TimeSpan.FromSeconds(30)};

        private static readonly Summary WritingBlockDuration =
            Metrics.CreateSummary("deepreader_storage_faster_write_block_duration", "Summary of time to store blocks to Faster", SummaryConfiguration);
        private static readonly Summary StoreLogMemorySizeBytesSummary =
           Metrics.CreateSummary("deepreader_storage_faster_block_store_log_memory_size_bytes", "Summary of the faster block store log memory size in bytes", SummaryConfiguration);
        private static readonly Summary StoreReadCacheMemorySizeBytesSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_read_cache_memory_size_bytes", "Summary of the faster block store read cache memory size in bytes", SummaryConfiguration);
        private static readonly Summary StoreEntryCountSummary =
           Metrics.CreateSummary("deepreader_storage_faster_block_store_read_cache_memory_size_bytes", "Summary of the faster block store entry count", SummaryConfiguration);
        private static readonly Summary StoreTakeLogCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_take_log_checkpoint_duration", "Summary of time to take a log checkpoint of faster block store", SummaryConfiguration);
        private static readonly Summary StoreTakeIndexCheckpointDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_take_index_checkpoint_duration", "Summary of time to take a index checkpoint of faster block store", SummaryConfiguration);
        private static readonly Summary StoreFlushAndEvictLogDurationSummary =
            Metrics.CreateSummary("deepreader_storage_faster_block_store_log_flush_and_evict_duration", "Summary of time to flush and evict faster block store", SummaryConfiguration);
        private static readonly Summary BlockReaderSessionReadDurationSummary =
          Metrics.CreateSummary("deepreader_storage_faster_block_get_by_id_duration", "Summary of time to try get block by id", SummaryConfiguration);

        public BlockStore(FasterStorageOptions options, ITopicEventSender eventSender, MetricsCollector metricsCollector)
        {
            _options = options;
            _eventSender = eventSender;
            _metricsCollector = metricsCollector;
            _metricsCollector.CollectMetricsHandler += CollectObservableMetrics;
            
            if (!_options.BlockStoreDir.EndsWith("/"))
                _options.BlockStoreDir += "/";


            // Create files for storing data
            var log = Devices.CreateLogDevice(_options.BlockStoreDir + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(_options.BlockStoreDir + "hlog.obj.log");
            
            // Define settings for log
            var logSettings = new LogSettings
            {
                LogDevice = log,
                ObjectLogDevice = objlog,
                ReadCacheSettings = options.UseReadCache ? new ReadCacheSettings() : null,
                // to calculate below:
                // 12 = 00001111 11111111 = 4095 = 4K
                // 34 = 11111111 11111111 11111111 11111111 = 17179869183 = 16G
                PageSizeBits = 14, // (16K pages)
                MemorySizeBits = 32 // (4G memory for main log)
            };
            
            // Define serializers; otherwise FASTER will use the slower DataContract
            // Needed only for class keys/values
            var serializerSettings = new SerializerSettings<long, Block>
            {
                valueSerializer = () => new BlockValueSerializer()
            };

            var checkPointsDir = _options.BlockStoreDir + "checkpoints";

            var checkpointManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(checkPointsDir), true);


            _store = new FasterKV<long, Block>(
                size: _options.MaxBlocksCacheEntries, // Cache Lines for Blocks
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointManager = checkpointManager },
                serializerSettings: serializerSettings
            );

            if (Directory.Exists(checkPointsDir))
            {
                try
                {
                    Log.Information("Recovering BlockStore");
                    _store.Recover(1);
                    Log.Information("BlockStore recovered");
                }
                catch (Exception e)
                {
                    Log.Error(e, "");
                    throw;
                }
            }

            //foreach (var recoverableSession in _store.RecoverableSessions)
            //{
            //    if (recoverableSession.Item2 == "BlockWriterSession")
            //    {
            //        _blockWriterSession = _store.For(new BlockFunctions())
            //            .ResumeSession<BlockFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
            //    }
            //    else if (recoverableSession.Item2 == "BlockReaderSession")
            //    {
            //        _blockReaderSession = _store.For(new BlockFunctions())
            //            .ResumeSession<BlockFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
            //    }
            //}

            //_blockWriterSession ??=
            //    _store.For(new BlockFunctions()).NewSession<BlockFunctions>("BlockWriterSession");
            //_blockReaderSession ??=
            //    _store.For(new BlockFunctions()).NewSession<BlockFunctions>("BlockReaderSession");

            StoreLogMemorySizeBytesSummary.Observe(_store.Log.MemorySizeBytes);
            if(options.UseReadCache)
                StoreReadCacheMemorySizeBytesSummary.Observe(_store.ReadCache.MemorySizeBytes);// must be optional
            StoreEntryCountSummary.Observe(_store.EntryCount);

            var blockEvictionObserver = new BlockEvictionObserver();
            _store.Log.SubscribeEvictions(blockEvictionObserver);

            if(options.UseReadCache)
                _store.ReadCache.SubscribeEvictions(blockEvictionObserver);

            _sessionPool = new AsyncPool<ClientSession<long, Block, BlockInput, BlockOutput, BlockContext, BlockFunctions>>(
                logSettings.LogDevice.ThrottleLimit,
                () => _store.For(new BlockFunctions()).NewSession<BlockFunctions>("BlockSession" + Interlocked.Increment(ref _sessionCount)));

            foreach (var recoverableSession in _store.RecoverableSessions)
            {
                _sessionPool.Return(_store.For(new BlockFunctions())
                    .ResumeSession<BlockFunctions>(recoverableSession.Item2, out CommitPoint commitPoint));
                _sessionCount++;
            }

            new Thread(CommitThread).Start();
        }

        public long BlocksIndexed => _store.EntryCount;

        private void CollectObservableMetrics(object? sender, EventArgs e)
        {
            StoreLogMemorySizeBytesSummary.Observe(_store.Log.MemorySizeBytes);
            if(_options.UseReadCache)
                StoreReadCacheMemorySizeBytesSummary.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountSummary.Observe(_store.EntryCount);
        }

        public async Task<Status> WriteBlock(Block block)
        {
            long blockId = block.Number;

            await _eventSender.SendAsync("BlockAdded", block);

            using (WritingBlockDuration.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var result = await session.UpsertAsync(ref blockId, ref block);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                _sessionPool.Return(session);
                return result.Status;
            }
        }

        public async Task<(bool, Block)> TryGetBlockById(uint blockNum)
        {
            using (BlockReaderSessionReadDurationSummary.NewTimer())
            {
                if (!_sessionPool.TryGet(out var session))
                    session = await _sessionPool.GetAsync().ConfigureAwait(false);
                var (status, output) = (await session.ReadAsync(blockNum)).Complete();
                _sessionPool.Return(session);
                return (status.Found, output.Value);
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