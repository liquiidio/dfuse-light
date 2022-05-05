using System.Diagnostics;
using System.Threading.Channels;
using DeepReader.Options;
using KGySoft.CoreLibraries;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Sentry;
using Serilog;

namespace DeepReader.HostedServices;

public class DlogReaderWorker : BackgroundService
{
    private readonly ChannelWriter<List<IList<StringSegment>>> _blockSegmentsListChannel;
    
    private readonly DeepMindProcess _deepMindProcess;
    
    private DeepReaderOptions _deepReaderOptions;
    private MindReaderOptions _mindReaderOptions;

    private readonly ObjectPool<List<IList<StringSegment>>> _blockSegmentListPool;
    private List<IList<StringSegment>> _blockSegmentsList;

    public DlogReaderWorker(ChannelWriter<List<IList<StringSegment>>> blockSegmentsListChannel,
        IOptionsMonitor<DeepReaderOptions> deepReaderOptionsMonitor,
        IOptionsMonitor<MindReaderOptions> mindReaderOptionsMonitor,
        ObjectPool<List<IList<StringSegment>>> blockSegmentListPool)
    {
        _blockSegmentsListChannel = blockSegmentsListChannel;

        _deepReaderOptions = deepReaderOptionsMonitor.CurrentValue;
        deepReaderOptionsMonitor.OnChange(OnDeepReaderOptionsChanged);

        _mindReaderOptions = mindReaderOptionsMonitor.CurrentValue;
        mindReaderOptionsMonitor.OnChange(OnMindReaderOptionsChanged);

        _blockSegmentListPool = blockSegmentListPool;
        _blockSegmentsList = _blockSegmentListPool.Get();

        var vars = Environment.GetEnvironmentVariables();
        if (vars.Contains("WSLENV") && vars.Contains("NAME") && (string)vars["NAME"] == "cmadh_desktop")
        {
            var mindreaderDir = "/home/cmadh/testing/config/";
            var dataDir = "/home/cmadh/testing/data/";

            _deepMindProcess = new DeepMindProcess(_mindReaderOptions, mindreaderDir, dataDir);
        }
        else if (vars.Contains("WSLENV") && vars.Contains("NAME") && (string)vars["NAME"] == "DESKTOP-JGBAUO9")
        {
            var mindreaderDir = "/home/haron/testing/config/";
            var dataDir = "/home/haron/testing/data/";

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
        // TODO, for some reason I need to manually call the Init
        SentrySdk.Init("https://b4874920c4484212bcc323e9deead2e9@sentry.noodles.lol/2");

        Thread.CurrentThread.Name = $"DlogReaderWorker";

        await StartNodeos(stoppingToken);
    }

    private async Task StartNodeos(CancellationToken clt)
    {
        _deepMindProcess.OutputDataReceived += OnNodeosDataReceived;
        _deepMindProcess.ErrorDataReceived += OnNodeosDataReceived;
        await _deepMindProcess.RunAsync(clt);
    }

    private void OnNodeosDataReceived(object sender, DataReceivedEventArgs e)
    {
        try
        {
            if (e.Data != null && e.Data.StartsWith("DMLOG"))
            {
                if (e.Data.StartsWith("DMLOG START_BLOCK"))
                {
                    _blockSegmentsListChannel.WriteAsync(_blockSegmentsList);
                    _blockSegmentsList = _blockSegmentListPool.Get();
                }
                _blockSegmentsList.Add(e.Data.AsSegment().Split(' ', 12));
            }
            else
                Log.Information(e.Data);
        }
        catch(Exception ex)
        {
            Log.Error(ex, "");
        }
    }
}