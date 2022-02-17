using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Types;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerSchedule
{
    public uint Version = 0;//uint32
    public ProducerKey[] Producers = Array.Empty<ProducerKey>();//[]*ProducerKey
}