using DeepReader.Types;
using Serilog;
using System.Text.Json;
using System.Threading.Channels;

namespace DeepReader
{
    public class BlockWorker : BackgroundService
    {
        private readonly ILogger<BlockWorker> _logger;

        private readonly ChannelReader<Block> _blocksChannel;

        public BlockWorker(ILogger<BlockWorker> logger, ChannelReader<Block> blocksChannel)
        {
            _logger = logger;
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
}
