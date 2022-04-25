namespace DeepReader.Configuration
{
    public class MindReaderOptions
    {
        public bool DeleteAllBlocks { get; set; }
        public string? ConfigDir { get; set; }
        public string? DataDir { get; set; }
        public string? GenesisJson { get; set; }
        public bool ReplayBlockchain { get; set; }
        public bool HardReplayBlockchain { get; set; }
        public string? Snapshot { get; set; }
        public bool ForceAllChecks { get; set; }
        public bool RedirectStandardError { get; set; } = true;
        public bool RedirectStandardOutput { get; set; } = true;
        public string? ProtocolFeaturesDir { get; set; }
    }
}
