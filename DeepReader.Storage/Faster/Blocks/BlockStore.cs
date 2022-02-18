using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.FlattenedTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.Blocks
{
    public class BlockStore
    {
        private FasterKV<BlockId, FlattenedBlock> store;

        private bool useReadCache = false;

        private readonly ClientSession<BlockId, FlattenedBlock, BlockInput, BlockOutput, BlockContext, BlockFunctions> _blockStoreSession;

        public BlockStore()
        {

            // Create files for storing data
            var path = Path.GetTempPath() + "ClassCache/";
            var log = Devices.CreateLogDevice(path + "hlog.log");

            // Log for storing serialized objects; needed only for class keys/values
            var objlog = Devices.CreateLogDevice(Path.GetTempPath() + "hlog.obj.log");

            // Define settings for log
            var logSettings = new LogSettings
            {
                LogDevice = log,
                ObjectLogDevice = objlog,
                ReadCacheSettings = useReadCache ? new ReadCacheSettings() : null,
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

            store = new FasterKV<BlockId, FlattenedBlock>(
                size: 1L << 20,
                logSettings: logSettings,
                checkpointSettings: new CheckpointSettings { CheckpointDir = path },
                serializerSettings: serializerSettings,
                comparer: new BlockId(0)
            );

            _blockStoreSession = store.For(new BlockFunctions()).NewSession<BlockFunctions>();
        }

        public async Task<Status> WriteBlock(FlattenedBlock block)
        {
            var blockId = new BlockId(block.Number);
            return (await _blockStoreSession.UpsertAsync(ref blockId, ref block)).Complete();
        }

    }
}
