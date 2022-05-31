using System.Text.Json.Serialization;

namespace DeepReader.Types;

public sealed class RlimitConfig : RlimitOp
{
    public ElasticLimitParameters CpuLimitParameters = new();//*ElasticLimitParameters
    public ElasticLimitParameters NetLimitParameters = new();//*ElasticLimitParameters
    public uint AccountCpuUsageAverageWindow = 0;//uint32
    public uint AccountNetUsageAverageWindow = 0;//uint32
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RlimitOpKind : byte
{
    CONFIG = 0,
    STATE = 1,
    ACCOUNT_LIMITS = 2,
    ACCOUNT_USAGE = 3
}