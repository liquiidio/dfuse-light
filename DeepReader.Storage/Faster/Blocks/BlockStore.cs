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

        private readonly ClientSession<long, Block, BlockInput, BlockOutput, BlockContext, BlockFunctions> _blockWriterSession;
        private readonly ClientSession<long, Block, BlockInput, BlockOutput, BlockContext, BlockFunctions> _blockReaderSession;

        private readonly FasterStorageOptions _options;


        private ITopicEventSender _eventSender;
        private MetricsCollector _metricsCollector;

        private static readonly Histogram WritingBlockDuration =
            Metrics.CreateHistogram("deepreader_storage_faster_write_block_duration", "Histogram of time to store blocks to Faster");
        private static readonly Histogram StoreLogMemorySizeBytesHistogram =
           Metrics.CreateHistogram("deepreader_storage_faster_block_store_log_memory_size_bytes", "Histogram of the faster block store log memory size in bytes");
        private static readonly Histogram StoreReadCacheMemorySizeBytesHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_block_store_read_cache_memory_size_bytes", "Histogram of the faster block store read cache memory size in bytes");
        private static readonly Histogram StoreEntryCountHistogram =
           Metrics.CreateHistogram("deepreader_storage_faster_block_store_read_cache_memory_size_bytes", "Histogram of the faster block store entry count");
        private static readonly Histogram StoreTakeLogCheckpointDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_block_store_take_log_checkpoint_duration", "Histogram of time to take a log checkpoint of faster block store");
        private static readonly Histogram StoreTakeIndexCheckpointDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_block_store_take_index_checkpoint_duration", "Histogram of time to take a index checkpoint of faster block store");
        private static readonly Histogram StoreFlushAndEvictLogDurationHistogram =
            Metrics.CreateHistogram("deepreader_storage_faster_block_store_log_flush_and_evict_duration", "Histogram of time to flush and evict faster block store");
        private static readonly Histogram BlockReaderSessionReadDurationHistogram =
          Metrics.CreateHistogram("deepreader_storage_faster_block_get_by_id_duration", "Histogram of time to try get block by id");

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
                Log.Information("Recovering BlockStore");
                _store.Recover(1);
                Log.Information("BlockStore recovered");
            }

            foreach (var recoverableSession in _store.RecoverableSessions)
            {
                if (recoverableSession.Item2 == "BlockWriterSession")
                {
                    _blockWriterSession = _store.For(new BlockFunctions())
                        .ResumeSession<BlockFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
                }
                else if (recoverableSession.Item2 == "BlockReaderSession")
                {
                    _blockReaderSession = _store.For(new BlockFunctions())
                        .ResumeSession<BlockFunctions>(recoverableSession.Item2, out CommitPoint commitPoint);
                }
            }

            _blockWriterSession ??=
                _store.For(new BlockFunctions()).NewSession<BlockFunctions>("BlockWriterSession");
            _blockReaderSession ??=
                _store.For(new BlockFunctions()).NewSession<BlockFunctions>("BlockReaderSession");

            StoreLogMemorySizeBytesHistogram.Observe(_store.Log.MemorySizeBytes);
            if(options.UseReadCache)
                StoreReadCacheMemorySizeBytesHistogram.Observe(_store.ReadCache.MemorySizeBytes);// must be optional
            StoreEntryCountHistogram.Observe(_store.EntryCount);

            var blockEvictionObserver = new BlockEvictionObserver();
            _store.Log.SubscribeEvictions(blockEvictionObserver);

            if(options.UseReadCache)
                _store.ReadCache.SubscribeEvictions(blockEvictionObserver);

            new Thread(CommitThread).Start();
        }

        public long BlocksIndexed => _store.EntryCount;

        private void CollectObservableMetrics(object? sender, EventArgs e)
        {
            StoreLogMemorySizeBytesHistogram.Observe(_store.Log.MemorySizeBytes);
            if(_options.UseReadCache)
                StoreReadCacheMemorySizeBytesHistogram.Observe(_store.ReadCache.MemorySizeBytes);
            StoreEntryCountHistogram.Observe(_store.EntryCount);
        }

        public async Task<Status> WriteBlock(Block block)
        {
            long blockId = block.Number;

            await _eventSender.SendAsync("BlockAdded", block);

            using (WritingBlockDuration.NewTimer())
            {
                var result = await _blockWriterSession.UpsertAsync(ref blockId, ref block);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                return result.Status;
            }
        }

        public async Task<(bool, Block)> TryGetBlockById(uint blockNum)
        {
            using (BlockReaderSessionReadDurationHistogram.NewTimer())
            {
                var (status, output) = (await _blockReaderSession.ReadAsync(blockNum)).Complete();
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

                    using (StoreTakeLogCheckpointDurationHistogram.NewTimer())
                        _store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver, true).GetAwaiter().GetResult();

                    if (_options.FlushAfterCheckpoint)
                    {
                        using (StoreFlushAndEvictLogDurationHistogram.NewTimer())
                            _store.Log.FlushAndEvict(true);
                    }

                    if (logCheckpointsTaken % _options.IndexCheckpointMultiplier == 0)
                    {
                        using (StoreTakeIndexCheckpointDurationHistogram.NewTimer())
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