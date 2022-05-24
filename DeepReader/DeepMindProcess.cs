using System.Diagnostics;
using System.Text;
using DeepReader.Options;
using Serilog;
using ZstdNet;

namespace DeepReader
{
    public class DeepMindProcess : Process
    {
        public DeepMindProcess(MindReaderOptions mindReaderOptions, string? mindReaderDir = null, string? dataDir = null)
        {
            if (!string.IsNullOrEmpty(mindReaderOptions.SnapshotName))
                DownloadSnapshot(mindReaderOptions.SnapshotUrl, mindReaderOptions.SnapshotDir, mindReaderOptions.SnapshotName).GetAwaiter().GetResult();

            this.StartInfo = new ProcessStartInfo()
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
            if (!this.Start())
            {
                Log.Error("Unable to start DeepMindProcess");//TODO (Corvin) better log
                return this.ExitCode;
            }

            if (StartInfo.RedirectStandardOutput)
                BeginOutputReadLine();

            if (StartInfo.RedirectStandardError)
                BeginErrorReadLine();

            await Task.Delay(5000, cancellationToken);

            //this.PriorityClass = ProcessPriorityClass.RealTime;
            this.PriorityBoostEnabled = true;

            await this.WaitForExitAsync(cancellationToken);
            return ExitCode;
        }

        public async Task<int> KillAsync(CancellationToken cancellationToken)
        {
            this.Kill(true);
            await this.WaitForExitAsync(cancellationToken);
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

            if (!string.IsNullOrEmpty(mindReaderOptions.Snapshot))
            {
                sb.Append($" --snapshot {mindReaderOptions.SnapshotName}");
                sb.Append($" --snapshots-dir {mindReaderOptions.SnapshotDir}");
            }
            else
            {

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
            }

            if (mindReaderOptions.ForceAllChecks)
                sb.Append(" --force-all-checks");

            if (dataDir == null && !string.IsNullOrEmpty(mindReaderOptions.ProtocolFeaturesDir))
                sb.Append($" --protocol-features-dir {mindReaderOptions.ProtocolFeaturesDir}");
            else if (mindReaderDir != null)
                sb.Append($" --protocol-features-dir {dataDir}");

            var argList = sb.ToString();

            Log.Information(argList);

            return argList;
        }

        private async Task DownloadSnapshot(string? snapshotUrl, string? snapshotDir, string? name)
        {
            if (snapshotUrl is null)
                throw new ArgumentNullException(nameof(snapshotUrl));

            if (snapshotDir is null)
                throw new ArgumentNullException(nameof(snapshotDir));

            var httpClient = new HttpClient();
            var result = await httpClient.GetByteArrayAsync(snapshotUrl);

            var data = DecompressSnapshot(result);

            try
            {
                Directory.Delete(snapshotDir, true);
                Directory.CreateDirectory(snapshotDir);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(snapshotDir);
            }
            SaveSnapshotToAFile($"{snapshotDir}/{name}", data);
        }

        private byte[] DecompressSnapshot(byte[] data)
        {
            using var decompressor = new Decompressor();
            return decompressor.Unwrap(data);
        }

        private void SaveSnapshotToAFile(string path, byte[] data)
        {
            using (var file = File.Create(path))
            {
                file.Write(data);
            }
        }
    }
}
