namespace DeepReader.Types;

public class RlimitAccountUsage : RlimitOp
{
    public string Owner = string.Empty;//string
    public UsageAccumulator NetUsage = new();//*UsageAccumulator
    public UsageAccumulator CpuUsage = new();//*UsageAccumulator
    public ulong RamUsage = 0;//uint64
}