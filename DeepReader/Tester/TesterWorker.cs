using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.Tester
{
    public class TesterWorker : BackgroundService
    {
        private readonly ILogger<TesterWorker> _logger;

        public TesterWorker(ILogger<TesterWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RunTests(stoppingToken);
        }

        private async Task RunTests(CancellationToken clt)
        {

        }

        private void PushTransactionViaCleos()
        {
            using var cleos = new Process();
            cleos.StartInfo = new ProcessStartInfo
            {
                FileName = "cleos",
                ArgumentList =
                {
                    "","",""
                },
                UseShellExecute = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                CreateNoWindow = true
            };
            cleos.Start();
        }
    }
}
