using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerSchedule
{
    public uint Version = 0;//uint32
    public ProducerKey[] Producers = Array.Empty<ProducerKey>();//[]*ProducerKey

    public static ProducerSchedule ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new ProducerSchedule()
        {
            Version = reader.ReadUInt32()
        };

        obj.Producers = new ProducerKey[reader.ReadInt32()];
        for (int i = 0; i < obj.Producers.Length; i++)
        {
            obj.Producers[i] = ProducerKey.ReadFromBinaryReader(reader);
        }

        return obj;
    }
}