using System.Text.Json;
using System.Threading.Channels;
using DeepReader.Types;
using Serilog;

namespace DeepReader.HostedServices;

public class BlockWorker : BackgroundService
{
    private readonly ChannelReader<Block> _blocksChannel;

    public BlockWorker(ChannelReader<Block> blocksChannel)
    {
        _blocksChannel = blocksChannel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ProcessBlocks(stoppingToken);
    }

    private async Task ProcessBlocks(CancellationToken clt)
    {
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            IncludeFields = true,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            MaxDepth = 0
        };

        await foreach (var block in _blocksChannel.ReadAllAsync(clt))
        {
            Log.Information($"got block {block.ToJsonString(jsonSerializerOptions)}");
        }
    }
}