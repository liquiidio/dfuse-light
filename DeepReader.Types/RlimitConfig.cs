namespace DeepReader.Types;

public class RlimitConfig : RlimitOp
{
    public ElasticLimitParameters CpuLimitParameters = new();//*ElasticLimitParameters
    public ElasticLimitParameters NetLimitParameters = new();//*ElasticLimitParameters
    public uint AccountCpuUsageAverageWindow = 0;//uint32
    public uint AccountNetUsageAverageWindow = 0;//uint32
}

public enum RlimitOpKind : byte
{
    CONFIG = 0,
    STATE = 1,
    ACCOUNT_LIMITS = 2,
    ACCOUNT_USAGE = 3
}