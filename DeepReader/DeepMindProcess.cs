using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Configuration;
using DeepReader.Options;
using Serilog;

namespace DeepReader
{
    public class DeepMindProcess : Process
    {
        public DeepMindProcess(MindReaderOptions mindReaderOptions)
        {
            this.StartInfo = new ProcessStartInfo()
            {
                FileName = "nodeos",
                Arguments = BuildArgumentList(mindReaderOptions),
                UseShellExecute = false,
                RedirectStandardError = mindReaderOptions.RedirectStandardError,
                RedirectStandardOutput = mindReaderOptions.RedirectStandardOutput,
                RedirectStandardInput = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
        }

        public async Task<int> RunAsync(CancellationToken cancellationToken)
        {
            if (!this.Start())
            {
                Log.Error("Unable to start DeepMindProcess");//TODO (Corvin) better log
                return this.ExitCode;
            }

            if (StartInfo.RedirectStandardOutput)
                BeginOutputReadLine();

            if (StartInfo.RedirectStandardError)
                BeginErrorReadLine();

            await this.WaitForExitAsync(cancellationToken);
            return ExitCode;
        }

        public async Task<int> KillAsync(CancellationToken cancellationToken)
        {
            this.Kill(true);
            await this.WaitForExitAsync(cancellationToken);
            return ExitCode;
        }

        public string BuildArgumentList(MindReaderOptions mindReaderOptions)
        {
            var sb = new StringBuilder();
            if (mindReaderOptions.DeleteAllBlocks)
                sb.Append(" --delete-all-blocks");

            if (!string.IsNullOrEmpty(mindReaderOptions.ConfigDir))
                sb.Append($"--config-dir {mindReaderOptions.ConfigDir}");
            else
                sb.Append("");//TODO default?

            if (!string.IsNullOrEmpty(mindReaderOptions.DataDir))
                sb.Append($" --data-dir {mindReaderOptions.DataDir}");
            else
                sb.Append("");//TODO default?

            if (!string.IsNullOrEmpty(mindReaderOptions.GenesisJson))
                sb.Append($" --genesis-json {mindReaderOptions.GenesisJson}");
            else
                sb.Append("");//TODO default if not replay?

            if (mindReaderOptions.ForceAllChecks)
                sb.Append(" --force-all-checks");

            if (mindReaderOptions.ReplayBlockchain)
                sb.Append(" --replay-blockchain");

            if (mindReaderOptions.HardReplayBlockchain)
                sb.Append(" --hard-replay-blockchain");

            if (!string.IsNullOrEmpty(mindReaderOptions.Snapshot))
                sb.Append($" --snapshot {mindReaderOptions.Snapshot}");

            return sb.ToString();
        }
    }
}
