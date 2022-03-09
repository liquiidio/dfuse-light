namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerAuthoritySchedule
{
    public uint Version = 0;
    public ProducerAuthority[] Producers = Array.Empty<ProducerAuthority>();

    public static ProducerAuthoritySchedule ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new ProducerAuthoritySchedule()
        {
            Version = reader.ReadUInt32()
        };

        obj.Producers = new ProducerAuthority[reader.ReadInt32()];
        for (int i = 0; i < obj.Producers.Length; i++)
        {
            obj.Producers[i] = ProducerAuthority.ReadFromBinaryReader(reader);
        }

        return obj;
    }
}