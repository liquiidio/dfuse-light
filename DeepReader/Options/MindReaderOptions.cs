﻿namespace DeepReader.Options
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
        public string? SnapshotName { get; set; }
        public string? SnapshotUrl { get; set; }
        public string? SnapshotDir { get; set; }
        public bool ForceAllChecks { get; set; }
        public bool RedirectStandardError { get; set; } = false;
        public bool RedirectStandardOutput { get; set; } = true;
        public string? ProtocolFeaturesDir { get; set; }
    }
}
