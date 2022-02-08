namespace DeepReader.Types;

public class ElasticLimitParameters
{
    public ulong Target = 0;//uint64
    public ulong Max = 0;//uint64
    public uint Periods = 0;//uint32
    public uint MaxMultiplier = 0;//uint32
    public Ratio ContractRate = new();//*Ratio
    public Ratio ExpandRate = new();//*Ratio
}