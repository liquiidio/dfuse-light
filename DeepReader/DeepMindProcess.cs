using System.Diagnostics;
using System.Text;
using DeepReader.Options;
using Serilog;

namespace DeepReader
{
    public class DeepMindProcess : Process
    {
        public DeepMindProcess(MindReaderOptions mindReaderOptions, string? mindReaderDir = null, string? dataDir = null)
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "nodeos",
                Arguments = BuildArgumentList(mindReaderOptions, mindReaderDir, dataDir),
                UseShellExecute = false,
                RedirectStandardError = mindReaderOptions.RedirectStandardError,
                RedirectStandardOutput = mindReaderOptions.RedirectStandardOutput,
                RedirectStandardInput = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                //            UseShellExecute = false,
                //            RedirectStandardError = true,
                //            RedirectStandardInput = false,
                //            RedirectStandardOutput = true,
                //            CreateNoWindow = true,
                //            WindowStyle = ProcessWindowStyle.Hidden,
            };
        }

        public async Task<int> RunAsync(CancellationToken cancellationToken)
        {
            if (!Start())
            {
                Log.Error("Unable to start DeepMindProcess");//TODO (Corvin) better log
                return ExitCode;
            }

            if (StartInfo.RedirectStandardOutput)
                BeginOutputReadLine();

            if (StartInfo.RedirectStandardError)
                BeginErrorReadLine();

            await Task.Delay(5000, cancellationToken);

            //this.PriorityClass = ProcessPriorityClass.RealTime;
            PriorityBoostEnabled = true;

            await WaitForExitAsync(cancellationToken);
            return ExitCode;
        }

        public async Task<int> KillAsync(CancellationToken cancellationToken)
        {
            Kill(true);
            await WaitForExitAsync(cancellationToken);
            return ExitCode;
        }

        public string BuildArgumentList(MindReaderOptions mindReaderOptions, string? mindReaderDir = null,
            string? dataDir = null)
        {
            var sb = new StringBuilder();
            if (mindReaderOptions.DeleteAllBlocks)
                sb.Append(" --delete-all-blocks");

            if (mindReaderDir == null && !string.IsNullOrEmpty(mindReaderOptions.ConfigDir))
                sb.Append($"--config-dir {mindReaderOptions.ConfigDir}");
            else if (mindReaderDir != null)
                sb.Append($"--config-dir {mindReaderDir}");
            else
                sb.Append(""); //TODO default?

            if (dataDir == null && !string.IsNullOrEmpty(mindReaderOptions.DataDir))
                sb.Append($" --data-dir {mindReaderOptions.DataDir}");
            else if (dataDir != null)
                sb.Append($" --data-dir {dataDir}");
            else
                sb.Append(""); //TODO default?

            if (!mindReaderOptions.ReplayBlockchain && !mindReaderOptions.HardReplayBlockchain)
            {
                if (mindReaderDir == null && !string.IsNullOrEmpty(mindReaderOptions.GenesisJson))
                    sb.Append($" --genesis-json {mindReaderOptions.GenesisJson}");
                else if (mindReaderDir != null)
                    sb.Append($" --genesis-json {mindReaderDir}genesis.json");
                else
                    sb.Append(""); //TODO mandatory empty if not replay and no blocks and no snapshot?
            }
            else if (mindReaderOptions.HardReplayBlockchain)
                sb.Append(" --hard-replay-blockchain");
            else if (mindReaderOptions.ReplayBlockchain)
                sb.Append(" --replay-blockchain");

            if (mindReaderOptions.ForceAllChecks)
                sb.Append(" --force-all-checks");

            if (!string.IsNullOrEmpty(mindReaderOptions.Snapshot))
                sb.Append($" --snapshot {mindReaderOptions.Snapshot}");

            if (dataDir == null && !string.IsNullOrEmpty(mindReaderOptions.ProtocolFeaturesDir))
                sb.Append($" --protocol-features-dir {mindReaderOptions.ProtocolFeaturesDir}");
            else if (mindReaderDir != null)
                sb.Append($" --protocol-features-dir {dataDir}");

            var argList = sb.ToString();

            Log.Information(argList);

            return argList;
        }
    }
}
