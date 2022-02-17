namespace DeepReader.Types;

public class RlimitState : RlimitOp
{
    public UsageAccumulator AverageBlockNetUsage = new();//*UsageAccumulator
    public UsageAccumulator AverageBlockCpuUsage = new();//*UsageAccumulator
    public ulong PendingNetUsage = 0;//uint64
    public ulong PendingCpuUsage = 0;//uint64
    public ulong TotalNetWeight = 0;//uint64
    public ulong TotalCpuWeight = 0;//uint64
    public ulong TotalRamBytes = 0;//uint64
    public ulong VirtualNetLimit = 0;//uint64
    public ulong VirtualCpuLimit = 0;//uint64
}