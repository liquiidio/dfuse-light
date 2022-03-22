namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerAuthoritySchedule : IEosioSerializable<ProducerAuthoritySchedule>
{
    public uint Version = 0;
    public ProducerAuthority[] Producers = Array.Empty<ProducerAuthority>();

    public static ProducerAuthoritySchedule ReadFromBinaryReader(BinaryReader reader)
    {
        var producerAuthoritySchedule = new ProducerAuthoritySchedule()
        {
            Version = reader.ReadUInt32()
        };

        producerAuthoritySchedule.Producers = new ProducerAuthority[reader.Read7BitEncodedInt()];
        for (int i = 0; i < producerAuthoritySchedule.Producers.Length; i++)
        {
            producerAuthoritySchedule.Producers[i] = ProducerAuthority.ReadFromBinaryReader(reader);
        }

        return producerAuthoritySchedule;
    }
}