using FASTER.server;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace DeepReader.Storage.Faster.Transactions
{
    public class TransactionStoreServer : BackgroundService
    {

        private readonly ServerOptions _serverOptions;
        private readonly VarLenServer _server;

        public TransactionStoreServer()
        {
            _serverOptions = new ServerOptions()
            {
                Port = 5003,
                Address = "127.0.0.1",
                MemorySize = "16g",
                PageSize = "32m",
                SegmentSize = "1g",
                IndexSize = "8g",
                EnableStorageTier = false,
                LogDir = null,
                CheckpointDir = null,
                Recover = false,
                DisablePubSub = false,
                PubSubPageSize = "4k"
            };
            _server = new VarLenServer(_serverOptions);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _server.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }

        public override void Dispose()
        {
            _server.Dispose();
            base.Dispose();
        }
    }
}