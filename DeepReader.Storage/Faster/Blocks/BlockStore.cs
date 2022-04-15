using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Storage.Options;
using DeepReader.Types;
using DeepReader.Types.FlattenedTypes;
 using FASTER.core;
using Prometheus;

namespace DeepReader.Storage.Faster.Blocks
{
    public class BlockStore
    {
        private readonly FasterKV<BlockId, FlattenedBlock> _store;

        private readonly ClientSession<BlockId, FlattenedBlock, BlockInput, BlockOutput, BlockContext, BlockFunctions> _blockStoreSession;

        private static readonly Histogram WritingBlockDuration = Metrics.CreateHistogram("deepreader_storage_faster_write_block_duration", "Histogram of time to store blocks to Faster");

        public BlockStore(FasterStorageOptions options){

            if (!options.BlockStoreDir.EndsWith("/"))
                options.BlockStoreDir += "/";

            // Create files for storing data
            var log = Devices.CreateLogDevice(options.BlockStoreDir + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(options.BlockStoreDir + "hlog.obj.log");

            // Define settings for log
            var logSettings = new LogSettings
            {
                LogDevice = log,
                ObjectLogDevice = objlog,
                ReadCacheSettings = options.UseReadCache ? new ReadCacheSettings() : null,
                // Uncomment below for low memory footprint demo
                // PageSizeBits = 12, // (4K pages)
                // MemorySizeBits = 20 // (1M memory for main log)
            };

            // Define serializers; otherwise FASTER will use the slower DataContract
            // Needed only for class keys/values
            var serializerSettings = new SerializerSettings<BlockId, FlattenedBlock>
            {
                keySerializer = () => new BlockIdSerializer(),
                valueSerializer = () => new BlockValueSerializer()
            };

            _store = new FasterKV<BlockId, FlattenedBlock>(
                size: options.MaxBlocksCacheEntries, // Cache Lines for Blocks
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointDir = options.BlockStoreDir },
                serializerSettings: serializerSettings,
                comparer: new BlockId(0)
            );

            _blockStoreSession = _store.For(new BlockFunctions()).NewSession<BlockFunctions>();
            new Thread(CommitThread).Start();
        }

        public async Task<Status> WriteBlock(FlattenedBlock block)
        {
            var blockId = new BlockId(block.Number);

            using (WritingBlockDuration.NewTimer())
            {
                return (await _blockStoreSession.UpsertAsync(ref blockId, ref block)).Complete();
            }
        }

        public async Task<(bool, FlattenedBlock)> TryGetBlockById(uint blockNum)
        {
            var (status, output) = (await _blockStoreSession.ReadAsync(new BlockId(blockNum))).Complete();
            return (status == Status.OK, output.Value);
        }

        private void CommitThread()
        {
            while (true)
            {
                Thread.Sleep(60000);

                // Take log-only checkpoint (quick - no index save)
                //store.TakeHybridLogCheckpointAsync(CheckpointType.FoldOver).GetAwaiter().GetResult();

                // Take index + log checkpoint (longer time)
                _store.TakeFullCheckpointAsync(CheckpointType.FoldOver).GetAwaiter().GetResult();
            }
        }
    }
}
