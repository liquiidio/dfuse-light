using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerSchedule : IEosioSerializable<ProducerSchedule>
{
    public uint Version = 0;//uint32
    public ProducerKey[] Producers = Array.Empty<ProducerKey>();//[]*ProducerKey

    public static ProducerSchedule ReadFromBinaryReader(BinaryReader reader)
    {
        var producerSchedule = new ProducerSchedule()
        {
            Version = reader.ReadUInt32()
        };

        producerSchedule.Producers = new ProducerKey[reader.Read7BitEncodedInt()];
        for (int i = 0; i < producerSchedule.Producers.Length; i++)
        {
            producerSchedule.Producers[i] = ProducerKey.ReadFromBinaryReader(reader);
        }

        return producerSchedule;
    }
}