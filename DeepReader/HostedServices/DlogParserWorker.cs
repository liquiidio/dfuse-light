using System.Threading.Channels;
using DeepReader.Classes;
using DeepReader.Options;
using DeepReader.Storage;
using DeepReader.Types;
using KGySoft.CoreLibraries;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Serilog;

namespace DeepReader.HostedServices;

public class DlogParserWorker : BackgroundService
{
    private readonly ChannelReader<List<IList<StringSegment>>> _blockSegmentsListChannel;
    private readonly ChannelWriter<Block> _blocksChannel;

    private DeepReaderOptions _deepReaderOptions;
    private MindReaderOptions _mindReaderOptions;

    private readonly ObjectPool<List<IList<StringSegment>>> _blockSegmentListPool;
    private readonly ObjectPool<Block> _blockPool;

    AbiDecoder _abiDecoder;

    public DlogParserWorker(ChannelReader<List<IList<StringSegment>>> blockSegmentsListChannel, ChannelWriter<Block> blocksChannel,
        IStorageAdapter storageAdapter,
        IOptionsMonitor<DeepReaderOptions> deepReaderOptionsMonitor,
        IOptionsMonitor<MindReaderOptions> mindReaderOptionsMonitor,
        ObjectPool<List<IList<StringSegment>>> blockSegmentListPool,
        ObjectPool<Block> blockPool)
    {
        _blockSegmentsListChannel = blockSegmentsListChannel;
        _blocksChannel = blocksChannel;

        _deepReaderOptions = deepReaderOptionsMonitor.CurrentValue;
        deepReaderOptionsMonitor.OnChange(OnDeepReaderOptionsChanged);

        _mindReaderOptions = mindReaderOptionsMonitor.CurrentValue;
        mindReaderOptionsMonitor.OnChange(OnMindReaderOptionsChanged);

        _blockSegmentListPool = blockSegmentListPool;
        _blockPool = blockPool;

        _abiDecoder = new AbiDecoder(storageAdapter);
    }

    private void OnDeepReaderOptionsChanged(DeepReaderOptions newOptions)
    {
        _deepReaderOptions = newOptions;
    }

    private void OnMindReaderOptionsChanged(MindReaderOptions newOptions)
    {
        _mindReaderOptions = newOptions;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var parseTasks = new List<Task>();
        for (var i = 0; i < _deepReaderOptions.DlogParserTasks; i++)
        {
            parseTasks.Add(ConsumeQueue(stoppingToken, i));
        }
        await Task.WhenAll(parseTasks);
    }

    private async Task ConsumeQueue(CancellationToken cancellationToken, int i)
    {
        uint blockNum = 0;
        var block = _blockPool.Get();
        Thread.CurrentThread.Name = $"ConsumeQueue {i}";
        ParseCtx ctx = new(_abiDecoder);
        await foreach (var logs in _blockSegmentsListChannel.ReadAllAsync(cancellationToken))
        {
            try
            {
                foreach (var data in logs)
                /*
                 * TODO can we parallelize this ?
                 * by running all operations followed by a
                 * APPLIED_TRANSACTION in parallel until we got the next APPLIED_TRANSACTION
                 * and by waiting for everything to finish before we process a ACCEPTED_BLOCK
                 *
                 * Having individual contexts per TransactionTrace could also work
                 */
                {
                    switch ((string) data[1]!)
                    {
                        case "RAM_OP":
                            ctx.ReadRamOp(data);
                            break;
                        case "CREATION_OP":
                            ctx.ReadCreationOp(data);
                            break;
                        case "DB_OP":
                            ctx.ReadDbOp(data);
                            break;
                        case "RLIMIT_OP":
                            ctx.ReadRlimitOp(data);
                            break;
                        case "TRX_OP":
                            ctx.ReadTrxOp(data);
                            break;
                        case "APPLIED_TRANSACTION":
                            ctx.ReadAppliedTransaction(data);
                            break;
                        case "TBL_OP":
                            ctx.ReadTableOp(data);
                            break;
                        case "PERM_OP":
                            ctx.ReadPermOp(data);
                            break;
                        case "DTRX_OP":
                            switch ((string) data[2]!)
                            {
                                case "CREATE":
                                    ctx.ReadCreateOrCancelDTrxOp("CREATE", data);
                                    break;
                                case "MODIFY_CREATE":
                                    ctx.ReadCreateOrCancelDTrxOp("MODIFY_CREATE", data);
                                    break;
                                case "MODIFY_CANCEL":
                                    ctx.ReadCreateOrCancelDTrxOp("MODIFY_CANCEL", data);
                                    break;
                                case "PUSH_CREATE":
                                    ctx.ReadCreateOrCancelDTrxOp("PUSH_CREATE", data);
                                    break;
                                case "CANCEL":
                                    ctx.ReadCreateOrCancelDTrxOp("CANCEL", data);
                                    break;
                                case "FAILED":
                                    ctx.ReadFailedDTrxOp(data);
                                    break;
                                default:
                                    Log.Information(data.ToString());
                                    break;
                            }

                            break;
                        case "RAM_CORRECTION_OP":
                            ctx.ReadRamCorrectionOp(data);
                            break;
                        case "ACCEPTED_BLOCK":
                            block = _blockPool.Get();
                            var acceptedBlock = ctx.ReadAcceptedBlock(data);
                            var blockWritten = _blocksChannel.TryWrite(acceptedBlock);
                            while (!blockWritten)
                            {
                                blockWritten = _blocksChannel.TryWrite(block);
                            }
                            blockNum = block.Number;
                            break;
                        case "START_BLOCK":
                            block = _blockPool.Get();
                            ctx.ReadStartBlock(data, block);
                            blockNum = block.Number;
                            break;
                        case "FEATURE_OP":
                            switch ((string) data[2]!)
                            {
                                case "PRE_ACTIVATE":
                                    ctx.ReadFeatureOpPreActivate(data);
                                    break;
                                case "ACTIVATE":
                                    ctx.ReadFeatureOpActivate(data);
                                    break;
                                default:
                                    Log.Information(data.ToString());
                                    break;
                            }

                            break;
                        case "SWITCH_FORK":
                            Log.Warning("fork signal, restarting state accumulation from beginning");
                            _blockPool.Return(block);
                            block = _blockPool.Get();
                            ctx.ResetBlock(block);
                            break;
                        case "ABIDUMP":
                            switch ((string) data[2]!)
                            {
                                case "START":
                                    ctx.ReadAbiStart(data);
                                    break;
                                case "ABI":
                                    ctx.ReadAbiDump(data);
                                    break;
                                case "END":
                                    ctx.AbiDumpEnd();
                                    break;
                            }

                            break;
                        case "DEEP_MIND_VERSION":
                            ctx.ReadDeepmindVersion(data);
                            break;
                        default:
                            Log.Information(data.ToString());
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, " at block {@blockNum} | {ctx.activeBlockNum}", blockNum, ctx.ActiveBlockNum);
                _blockPool.Return(block);
            }
            finally
            {
                _blockSegmentListPool.Return(logs);
            }
        }
    }
}