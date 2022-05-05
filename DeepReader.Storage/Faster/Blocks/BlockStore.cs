using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Storage.Faster.Transactions;
using DeepReader.Storage.Options;
using DeepReader.Types;
using DeepReader.Types.FlattenedTypes;
 using FASTER.core;
using Prometheus;
using Sentry;
using Serilog;

namespace DeepReader.Storage.Faster.Blocks
{
    public class BlockStore
    {
        private readonly FasterKV<BlockId, FlattenedBlock> _store;

        private readonly ClientSession<BlockId, FlattenedBlock, BlockInput, BlockOutput, BlockContext, BlockFunctions> _blockWriterSession;
        private readonly ClientSession<BlockId, FlattenedBlock, BlockInput, BlockOutput, BlockContext, BlockFunctions> _blockReaderSession;

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
                // to calculate below:
                // 12 = 00001111 11111111 = 4095 = 4K
                // 34 = 11111111 11111111 11111111 11111111 = 17179869183 = 16G
                PageSizeBits = 12, // (4K pages)
                MemorySizeBits = 32 // (4G memory for main log)
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

//            _store.Log.SubscribeEvictions(new BlockEvictionObserver());

            // TODO, for some reason I need to manually call the Init
            SentrySdk.Init("https://b4874920c4484212bcc323e9deead2e9@sentry.noodles.lol/2");

            new Thread(CommitThread).Start();
        }

        public async Task<Status> WriteBlock(FlattenedBlock block)
        {
            var blockId = new BlockId(block.Number);

            using (WritingBlockDuration.NewTimer())
            {
                var result = await _blockWriterSession.UpsertAsync(ref blockId, ref block);
                while (result.Status.IsPending)
                    result = await result.CompleteAsync();
                return result.Status;
            }
        }

        public async Task<(bool, FlattenedBlock)> TryGetBlockById(uint blockNum)
        {
            var (status, output) = (await _blockReaderSession.ReadAsync(new BlockId(blockNum))).Complete();
            return (status.Found, output.Value);
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
                    _store.TakeFullCheckpointAsync(CheckpointType.FoldOver).GetAwaiter().GetResult();
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
