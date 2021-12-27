namespace DeepReader.Types;

public class ProducerAuthoritySchedule
{
    public uint Version = 0;//uint32
    public ProducerAuthority[] Producers = Array.Empty<ProducerAuthority>();//[]*ProducerAuthority
}