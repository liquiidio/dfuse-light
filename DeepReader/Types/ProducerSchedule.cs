namespace DeepReader.Types;

public class ProducerSchedule
{
    public uint Version = 0;//uint32
    public ProducerKey[] Producers = Array.Empty<ProducerKey>();//[]*ProducerKey
}