namespace DeepReader.Types;

public class RlimitConfig : RlimitOp
{
    public ElasticLimitParameters CpuLimitParameters = new ElasticLimitParameters();//*ElasticLimitParameters
    public ElasticLimitParameters NetLimitParameters = new ElasticLimitParameters();//*ElasticLimitParameters
    public uint AccountCpuUsageAverageWindow = 0;//uint32
    public uint AccountNetUsageAverageWindow = 0;//uint32
}