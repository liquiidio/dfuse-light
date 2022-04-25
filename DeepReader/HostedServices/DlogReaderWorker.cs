using System.Diagnostics;
using System.Threading.Channels;
using DeepReader.Configuration;
using DeepReader.Options;
using KGySoft.CoreLibraries;
using Microsoft.Extensions.Options;
using Serilog;

namespace DeepReader.HostedServices;

public class DlogReaderWorker : BackgroundService
{
    private readonly ChannelWriter<IList<IList<StringSegment>>> _dlogChannel;

    private readonly DeepMindProcess _deepMindProcess;
    private DeepReaderOptions _deepReaderOptions;
    private MindReaderOptions _mindReaderOptions;

    public DlogReaderWorker(ChannelWriter<IList<IList<StringSegment>>> dlogChannel,
        IOptionsMonitor<DeepReaderOptions> deepReaderOptionsMonitor,
        IOptionsMonitor<MindReaderOptions> mindReaderOptionsMonitor)
    {
        _dlogChannel = dlogChannel;

        _deepReaderOptions = deepReaderOptionsMonitor.CurrentValue;
        deepReaderOptionsMonitor.OnChange(OnDeepReaderOptionsChanged);

        _mindReaderOptions = mindReaderOptionsMonitor.CurrentValue;
        mindReaderOptionsMonitor.OnChange(OnMindReaderOptionsChanged);

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
        Thread.CurrentThread.Name = $"DlogReaderWorker";

        await StartNodeos(stoppingToken);
    }

    private async Task StartNodeos(CancellationToken clt)
    {
        // TODO (Corvin) check nodeos version, provide arguments-list

        await Task.Delay(3000, clt);

        _deepMindProcess.OutputDataReceived += OnNodeosDataReceived;
        await _deepMindProcess.RunAsync(clt);
    }

    private void OnNodeosExited(object? sender, EventArgs e)
    {
        Log.Warning("Nodeos exited2");
    }

    IList<IList<StringSegment>> blockLogs = new List<IList<StringSegment>>(20000);
    private void OnNodeosDataReceived(object sender, DataReceivedEventArgs e)
    {
        try
        {
            if (e.Data != null && e.Data.StartsWith("DMLOG"))
            {
                if (e.Data.StartsWith("DMLOG START_BLOCK"))
                {
                    //while (!_dlogChannel.TryWrite(blockLogs))
                    //{
                    //    if (_dlogChannel.TryWrite(blockLogs))
                    //        break;
                    //}
                    while (!_dlogChannel.TryWrite(blockLogs))
                    {
                        if(_dlogChannel.TryWrite(blockLogs))
                            break;
                    }
                    blockLogs = new List<IList<StringSegment>>(20000);
                }
                blockLogs.Add(e.Data.AsSegment().Split(' ', 12));
            }
        }
        catch(Exception ex)
        {
            Log.Error(ex, "");
        }
    }
}