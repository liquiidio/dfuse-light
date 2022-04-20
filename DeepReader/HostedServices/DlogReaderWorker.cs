using System.Diagnostics;
using System.Threading.Channels;
using DeepReader.Classes;
using DeepReader.Configuration;
using DeepReader.Options;
using DeepReader.Types;
using Microsoft.Extensions.Options;
using Serilog;

namespace DeepReader.HostedServices;

public class DlogReaderWorker : BackgroundService
{
    private readonly ChannelWriter<Block> _blocksChannel;

    private DeepMindProcess _deepMindProcess;
    private DeepReaderOptions _deepReaderOptions;
    private MindReaderOptions _mindReaderOptions;
    readonly ParseCtx _ctx = new();

    public DlogReaderWorker(ChannelWriter<Block> blocksChannel, IOptionsMonitor<DeepReaderOptions> _deepReaderOptionsMonitor, IOptionsMonitor<MindReaderOptions> _mindReaderOptionsMonitor)
    {
        _blocksChannel = blocksChannel;

        _deepReaderOptions = _deepReaderOptionsMonitor.CurrentValue;
        _deepReaderOptionsMonitor.OnChange(OnDeepReaderOptionsChanged);

        _mindReaderOptions = _mindReaderOptionsMonitor.CurrentValue;
        _mindReaderOptionsMonitor.OnChange(OnMindReaderOptionsChanged);

        _deepMindProcess = new DeepMindProcess(_mindReaderOptions);
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

        
        string mindreaderDir = "/home/cmadh/testing/config/";
        string dataDir = "/home/cmadh/testing/data/";

        //string mindreaderDir = "/app/config/mindreader/";
        //string dataDir = "/app/config/mindreader/data";

        using var mindreader = new Process();
        mindreader.StartInfo = new ProcessStartInfo
        {            
            FileName = "nodeos",
            ArgumentList =
            {
                /*"-e" ,"-p", "eosio",*/ "--delete-all-blocks", "--config-dir", $"{mindreaderDir}", "--protocol-features-dir", $"{dataDir}", "--data-dir", $"{dataDir}", "--genesis-json", $"{mindreaderDir}genesis.json",
//                    "-e -p eosio --delete-all-blocks --deep-mind --plugin eosio::producer_plugin --plugin eosio::producer_api_plugin --plugin eosio::chain_api_plugin --plugin eosio::http_plugin --plugin eosio::history_plugin --plugin eosio::history_api_plugin --filter-on='*' --access-control-allow-origin='*' --contracts-console --http-validate-host=false --verbose-http-errors"// >> nodeos.log 2 > &1 &
            }, // { "--delete-all-blocks --deep-mind" },
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            //                WorkingDirectory = "/usr/bin",
        };

        if (mindreader != null)
        {
            mindreader.OutputDataReceived += OnNodeosDataReceived; // async (sender, e) => await OnNodeosOutputDataReceived(sender, e, stoppingToken);
            mindreader.ErrorDataReceived += OnNodeosDataReceived;
            mindreader.Exited += OnNodeosExited;

            mindreader.Start();
            mindreader.BeginErrorReadLine();
            mindreader.BeginOutputReadLine();
            //await process.WaitForExitAsync(stoppingToken);
            while (!mindreader.HasExited)
            {
                await Task.Delay(10000, clt);

                if(mindreader.HasExited)
                {
                    Log.Warning("MINDREADER EXITED!");
                }
            }
            Log.Warning("EXITED 1");
        }
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
                    var data = e.Data.Split(' ');

                    switch (data[1])
                    {
                        case "RAM_OP":
                            _ctx.ReadRamOp(data[Range.StartAt(2)]);
                            break;
                        case "CREATION_OP":
                            _ctx.ReadCreationOp(data[Range.StartAt(2)]);
                            break;
                        case "DB_OP":
                            _ctx.ReadDbOp(data[Range.StartAt(2)]);
                            break;
                        case "RLIMIT_OP":
                            _ctx.ReadRlimitOp(data[Range.StartAt(2)]);
                            break;
                        case "TRX_OP":
                            _ctx.ReadTrxOp(data[Range.StartAt(2)]);
                            break;
                        case "APPLIED_TRANSACTION":
                            _ctx.ReadAppliedTransaction(data[Range.StartAt(2)]);
                            break;
                        case "TBL_OP":
                            _ctx.ReadTableOp(data[Range.StartAt(2)]);
                            break;
                        case "PERM_OP":
                            _ctx.ReadPermOp(data[Range.StartAt(2)]);
                            break;
                        case "DTRX_OP CREATE":
                            _ctx.ReadCreateOrCancelDTrxOp("CREATE", data[Range.StartAt(2)]);
                            break;
                        case "DTRX_OP MODIFY_CREATE":
                            _ctx.ReadCreateOrCancelDTrxOp("MODIFY_CREATE", data[Range.StartAt(2)]);
                            break;
                        case "DTRX_OP MODIFY_CANCEL":
                            _ctx.ReadCreateOrCancelDTrxOp("MODIFY_CANCEL", data[Range.StartAt(2)]);
                            break;
                        case "RAM_CORRECTION_OP":
                            _ctx.ReadRamCorrectionOp(data[Range.StartAt(2)]);
                            break;
                        case "DTRX_OP PUSH_CREATE":
                            _ctx.ReadCreateOrCancelDTrxOp("PUSH_CREATE", data[Range.StartAt(2)]);
                            break;
                        case "DTRX_OP CANCEL":
                            _ctx.ReadCreateOrCancelDTrxOp("CANCEL", data[Range.StartAt(2)]);
                            break;
                        case "DTRX_OP FAILED":
                            _ctx.ReadFailedDTrxOp(data[Range.StartAt(2)]);
                            break;
                        case "ACCEPTED_BLOCK":
                            var block = _ctx.ReadAcceptedBlock(data[Range.StartAt(2)]);
                            bool blockWritten = _blocksChannel.TryWrite(block);
                            while (!blockWritten)
                            {
                                blockWritten = _blocksChannel.TryWrite(block);
                            }
                            break;
                        case "START_BLOCK":
                            _ctx.ReadStartBlock(data[Range.StartAt(2)]);
                            break;
                        case "FEATURE_OP ACTIVATE":
                            _ctx.ReadFeatureOpActivate(data[Range.StartAt(2)]);
                            break;
                        case "FEATURE_OP":
                            switch (data[2])
                            {
                                case "PRE_ACTIVATE":
                                    _ctx.ReadFeatureOpPreActivate(data[Range.StartAt(2)]);
                                    break;
                            }
                            break;
                        case "SWITCH_FORK":
                            Log.Warning("fork signal, restarting state accumulation from beginning");
                            _ctx.ResetBlock();
                            break;
                        case "ABIDUMP":
                            switch (data[2])
                            {
                                case "START":
                                    _ctx.ReadAbiStart(data[Range.StartAt(3)]);
                                    break;
                                case "ABI":
                                    _ctx.ReadAbiDump(data[Range.StartAt(3)]);
                                    break;
                                case "END":
                                    // noop
                                    break;
                            }
                            break;
                        case "DEEP_MIND_VERSION":
                            _ctx.ReadDeepmindVersion(data[Range.StartAt(2)]);
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