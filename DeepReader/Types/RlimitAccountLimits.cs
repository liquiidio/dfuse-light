namespace DeepReader.Types;

public class RlimitAccountLimits : RlimitOp
{
    public string Owner = string.Empty;//string
    public bool Pending = false;//bool
    public long NetWeight = 0;//int64
    public long CpuWeight = 0;//int64
    public long RamBytes = 0;//int64
}