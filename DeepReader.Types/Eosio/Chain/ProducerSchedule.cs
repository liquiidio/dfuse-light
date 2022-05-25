using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerSchedule : IEosioSerializable<ProducerSchedule>
{
    public uint Version { get; set; }//uint32
    public ProducerKey[] Producers { get; set; }//[]*ProducerKey

    public ProducerSchedule(BinaryReader reader)
    {
        Version = reader.ReadUInt32();

        Producers = new ProducerKey[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Producers.Length; i++)
        {
            Producers[i] = ProducerKey.ReadFromBinaryReader(reader);
        }
    }

    public static ProducerSchedule ReadFromBinaryReader(BinaryReader reader)
    {
        return new ProducerSchedule(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Version);

        writer.Write7BitEncodedInt(Producers.Length);
        foreach (var producer in Producers)
        {
            producer.WriteToBinaryWriter(writer);
        }
    }
}