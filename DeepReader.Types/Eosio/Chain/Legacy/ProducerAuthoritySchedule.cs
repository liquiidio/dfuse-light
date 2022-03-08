namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerAuthoritySchedule
{
    public uint Version = 0;
    public ProducerAuthority[] Producers = Array.Empty<ProducerAuthority>();
}