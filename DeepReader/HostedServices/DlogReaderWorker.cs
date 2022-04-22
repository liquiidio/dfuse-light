using System.Diagnostics;
using System.Threading.Channels;
using DeepReader.Classes;
using DeepReader.Configuration;
using DeepReader.Options;
using DeepReader.Types;
using KGySoft.CoreLibraries;
using Microsoft.Extensions.Options;
using Microsoft.Toolkit.HighPerformance;
using Serilog;

namespace DeepReader.HostedServices;

public class DlogReaderWorker : BackgroundService
{
    private readonly ChannelWriter<Block> _blocksChannel;

    private DeepMindProcess _deepMindProcess;
    private DeepReaderOptions _deepReaderOptions;
    private MindReaderOptions _mindReaderOptions;
    private readonly ParseCtx _ctx = new();

    public DlogReaderWorker(ChannelWriter<Block> blocksChannel, IOptionsMonitor<DeepReaderOptions> _deepReaderOptionsMonitor, IOptionsMonitor<MindReaderOptions> _mindReaderOptionsMonitor)
    {
        _blocksChannel = blocksChannel;

        _deepReaderOptions = _deepReaderOptionsMonitor.CurrentValue;
        _deepReaderOptionsMonitor.OnChange(OnDeepReaderOptionsChanged);

        _mindReaderOptions = _mindReaderOptionsMonitor.CurrentValue;
        _mindReaderOptionsMonitor.OnChange(OnMindReaderOptionsChanged);

        var vars = Environment.GetEnvironmentVariables();
        if (vars.Contains("WSLENV") && vars.Contains("NAME") && (string)vars["NAME"] == "cmadh_desktop")
        {
            var mindreaderDir = "/home/cmadh/testing/config/";
            var dataDir = "/home/cmadh/testing/data/";

            _deepMindProcess = new DeepMindProcess(_mindReaderOptions, mindreaderDir, dataDir);
        }
        else if (vars.Contains("WSLENV") && vars.Contains("NAME") && (string)vars["NAME"] == "HARON_PC_NAME")
        {
            var mindreaderDir = "/home/cmadh/testing/config/";
            var dataDir = "/home/cmadh/testing/data/";

            _deepMindProcess = new DeepMindProcess(_mindReaderOptions, mindreaderDir, dataDir);
        }
        else if (vars.Contains("DOTNET_RUNNING_IN_CONTAINER"))
        {
            var mindreaderDir = "/app/config/mindreader/";
            var dataDir = "/app/config/mindreader/data";

            _deepMindProcess = new DeepMindProcess(_mindReaderOptions, mindreaderDir, dataDir);
        }
        else
        {
            _deepMindProcess = new DeepMindProcess(_mindReaderOptions);
        }
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
        // TODO Corvin       await _deepMindProcess.RunAsync(stoppingToken);
        await StartNodeos(stoppingToken);
    }

    private async Task StartNodeos(CancellationToken clt)
    {
        // TODO (Corvin) check nodeos version, provide arguments-list

        await Task.Delay(3000, clt);

        _deepMindProcess.OutputDataReceived += OnNodeosDataReceived; // async (sender, e) => await OpanNodeosOutputDataReceived(sender, e, stoppingToken);
        _deepMindProcess.ErrorDataReceived += OnNodeosDataReceived;
        await _deepMindProcess.RunAsync(clt);
    }

    private void OnNodeosExited(object? sender, EventArgs e)
    {
        Log.Warning("Nodeos exited2");
    }

    private void OnNodeosDataReceived(object sender, DataReceivedEventArgs e)
    {
        try
        {
            if (e.Data != null)
            {
                if (e.Data.StartsWith("DMLOG"))
                {
//                    var data = e.Data.Split(' ', 12);

                    var data = e.Data.AsSegment().Split(' ', 12);
//                    var a = dat[0] == "sf";

                    switch ((string)data[1]!)
                    {
                        case "RAM_OP":
                            _ctx.ReadRamOp(data);
                            break;
                        case "CREATION_OP":
                            _ctx.ReadCreationOp(data);
                            break;
                        case "DB_OP":
                            _ctx.ReadDbOp(data);
                            break;
                        case "RLIMIT_OP":
                            _ctx.ReadRlimitOp(data);
                            break;
                        case "TRX_OP":
                            _ctx.ReadTrxOp(data);
                            break;
                        case "APPLIED_TRANSACTION":
                            _ctx.ReadAppliedTransaction(data);
                            break;
                        case "TBL_OP":
                            _ctx.ReadTableOp(data);
                            break;
                        case "PERM_OP":
                            _ctx.ReadPermOp(data);
                            break;
                        case "DTRX_OP":
                            switch ((string)data[2]!)
                            {
                                case "CREATE":
                                    _ctx.ReadCreateOrCancelDTrxOp("CREATE", data);
                                    break;
                                case "MODIFY_CREATE":
                                    _ctx.ReadCreateOrCancelDTrxOp("MODIFY_CREATE", data);
                                    break;
                                case "MODIFY_CANCEL":
                                    _ctx.ReadCreateOrCancelDTrxOp("MODIFY_CANCEL", data);
                                    break;
                                case "PUSH_CREATE":
                                    _ctx.ReadCreateOrCancelDTrxOp("PUSH_CREATE", data);
                                    break;
                                case "CANCEL":
                                    _ctx.ReadCreateOrCancelDTrxOp("CANCEL", data);
                                    break;
                                case "FAILED":
                                    _ctx.ReadFailedDTrxOp(data);
                                    break;
                                default:
                                    Log.Information(e.Data);
                                    break;
                            }
                            break;
                        case "RAM_CORRECTION_OP":
                            _ctx.ReadRamCorrectionOp(data);
                            break;
                        case "ACCEPTED_BLOCK":
                            var block = _ctx.ReadAcceptedBlock(data);
                            var blockWritten = _blocksChannel.TryWrite(block);
                            while (!blockWritten)
                            {
                                blockWritten = _blocksChannel.TryWrite(block);
                            }
                            break;
                        case "START_BLOCK":
                            _ctx.ReadStartBlock(data);
                            break;
                        case "FEATURE_OP":
                            switch ((string)data[2]!)
                            {
                                case "PRE_ACTIVATE":
                                    _ctx.ReadFeatureOpPreActivate(data);
                                    break;
                                case "ACTIVATE":
                                    _ctx.ReadFeatureOpActivate(data);
                                    break;
                                default:
                                    Log.Information(e.Data);
                                    break;
                            }
                            break;
                        case "SWITCH_FORK":
                            Log.Warning("fork signal, restarting state accumulation from beginning");
                            _ctx.ResetBlock();
                            break;
                        case "ABIDUMP":
                            switch ((string)data[2]!)
                            {
                                case "START":
                                    _ctx.ReadAbiStart(data);
                                    break;
                                case "ABI":
                                    _ctx.ReadAbiDump(data);
                                    break;
                                case "END":
                                    // noop
                                    break;
                            }
                            break;
                        case "DEEP_MIND_VERSION":
                            _ctx.ReadDeepmindVersion(data);
                            break;
                        default:
                            Log.Information(e.Data);
                            break;
                        //zlog.Info("unknown log line", zap.String("line", data));
                    } 
                }
                //else
                //    Log.Information(e.Data);
            }
            else
                Log.Warning("data is null");
        }
        catch(Exception ex)
        {
            Log.Error(ex, "");
        }
    }
}