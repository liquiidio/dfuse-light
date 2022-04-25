using System.Threading.Channels;
using DeepReader.Classes;
using DeepReader.Configuration;
using DeepReader.Options;
using DeepReader.Types;
using KGySoft.CoreLibraries;
using Microsoft.Extensions.Options;
using Serilog;

namespace DeepReader.HostedServices;

public class DlogParserWorker : BackgroundService
{
    private readonly ChannelReader<IList<IList<StringSegment>>> _dlogChannel;
    private readonly ChannelWriter<Block> _blocksChannel;

    private DeepReaderOptions _deepReaderOptions;
    private MindReaderOptions _mindReaderOptions;

    public DlogParserWorker(ChannelReader<IList<IList<StringSegment>>> dlogChannel, ChannelWriter<Block> blocksChannel,
        IOptionsMonitor<DeepReaderOptions> deepReaderOptionsMonitor,
        IOptionsMonitor<MindReaderOptions> mindReaderOptionsMonitor)
    {
        _dlogChannel = dlogChannel;
        _blocksChannel = blocksChannel;

        _deepReaderOptions = deepReaderOptionsMonitor.CurrentValue;
        deepReaderOptionsMonitor.OnChange(OnDeepReaderOptionsChanged);

        _mindReaderOptions = mindReaderOptionsMonitor.CurrentValue;
        mindReaderOptionsMonitor.OnChange(OnMindReaderOptionsChanged);
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
        List<Task> parseTasks = new List<Task>();
        for (int i = 0; i < 8; i++)
        {
            parseTasks.Add(ConsumeQueue(stoppingToken, i));
        }
        await Task.WhenAll(parseTasks);
    }

    private async Task ConsumeQueue(CancellationToken cancellationToken, int i)
    {
        Thread.CurrentThread.Name = $"ConsumeQueue {i}";
        ParseCtx ctx = new();
        await foreach (var logs in _dlogChannel.ReadAllAsync(cancellationToken))
        {
            try
            {
                foreach (var data in logs)
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
                            var block = ctx.ReadAcceptedBlock(data);
                            var blockWritten = _blocksChannel.TryWrite(block);
                            while (!blockWritten)
                            {
                                blockWritten = _blocksChannel.TryWrite(block);
                            }

                            break;
                        case "START_BLOCK":
                            ctx.ReadStartBlock(data);
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
                            ctx.ResetBlock();
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
                                    // noop
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
                Log.Error(e, "");
            }
        }
    }
}