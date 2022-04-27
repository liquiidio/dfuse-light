using DeepReader.Storage.Options;
using DeepReader.Types.FlattenedTypes;
 using FASTER.core;
using Prometheus;
using Serilog;

namespace DeepReader.Storage.Faster.Blocks
{
    public class BlockStore
    {
        private readonly FasterKV<BlockId, FlattenedBlock> _store;

        private readonly ClientSession<BlockId, FlattenedBlock, BlockInput, BlockOutput, BlockContext, BlockFunctions> _blockStoreWriterSession;
        private readonly ClientSession<BlockId, FlattenedBlock, BlockInput, BlockOutput, BlockContext, BlockFunctions> _blockStoreReaderSession;

        private readonly BlockEvictionObserver _lockEvictionObserver;

        private FasterStorageOptions _options;

        private static readonly Histogram WritingBlockDuration = Metrics.CreateHistogram("deepreader_storage_faster_write_block_duration", "Histogram of time to store blocks to Faster");
        
        public BlockStore(FasterStorageOptions options)
        {
            _options = options;

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
                // Uncomment below for low memory footprint demo
                PageSizeBits = 12, // (4K pages)
                // MemorySizeBits = 20 // (1M memory for main log)
            };

            // Define serializers; otherwise FASTER will use the slower DataContract
            // Needed only for class keys/values
            var serializerSettings = new SerializerSettings<BlockId, FlattenedBlock>
            {
                keySerializer = () => new BlockIdSerializer(),
                valueSerializer = () => new BlockValueSerializer()
            };

            var checkPointsDir = _options.BlockStoreDir + "checkpoints";

            var checkpointManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(checkPointsDir), true);

            _store = new FasterKV<BlockId, FlattenedBlock>(
                size: _options.MaxBlocksCacheEntries, // Cache Lines for Blocks
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointManager = checkpointManager },
                serializerSettings: serializerSettings,
                comparer: new BlockId(0)
            );

            if (Directory.Exists(checkPointsDir))
            { 
                Log.Information("Recovering BlockStore");
                _store.Recover(1);
                Log.Information("BlockStore recovered");
            }

            _blockStoreWriterSession = _store.For(new BlockFunctions()).NewSession<BlockFunctions>();
            _blockStoreReaderSession = _store.For(new BlockFunctions()).NewSession<BlockFunctions>();

            _lockEvictionObserver = new BlockEvictionObserver(_store);

            new Thread(CommitThread).Start();
        }

        public async Task<Status> WriteBlock(FlattenedBlock block)
        {
            var blockId = new BlockId(block.Number);

            using (WritingBlockDuration.NewTimer())
            {
                var result = await _blockStoreWriterSession.UpsertAsync(ref blockId, ref block);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                return result.Status;
            }
        }

        public async Task<(bool, FlattenedBlock)> TryGetBlockById(uint blockNum)
        {
            var (status, output) = (await _blockStoreReaderSession.ReadAsync(new BlockId(blockNum))).Complete();
            return (status.IsCompletedSuccessfully, output.Value);
        }

        private void CommitThread()
        {
            if (_options.CheckpointInterval is null or 0)
                return;
            while (true)
            {
                Thread.Sleep(_options.CheckpointInterval.Value);

                // Take log-only checkpoint (quick - no index save)
                //store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver).GetAwaiter().GetResult();

                // Take index + log checkpoint (longer time)
                _store.TakeFullCheckpointAsync(CheckpointType.FoldOver).GetAwaiter().GetResult();
            }
        }
    }
}
