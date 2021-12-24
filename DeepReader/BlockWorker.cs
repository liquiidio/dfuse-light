using DeepReader.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DeepReader
{
    public class BlockWorker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly ChannelReader<Block> _blocksChannel;

        public BlockWorker(ILogger<Worker> logger, ChannelReader<Block> blocksChannel)
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
            await foreach (var block in _blocksChannel.ReadAllAsync(clt))
            {
                Console.WriteLine($"got block {block.Number}");
            }
        }
    }
}
