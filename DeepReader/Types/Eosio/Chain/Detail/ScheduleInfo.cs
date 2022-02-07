using DeepReader.EosTypes;
using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Types.Eosio.Chain.Detail;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public struct ScheduleInfo
{
    public uint ScheduleLibNum = 0; /// last irr block num
    public DigestType ScheduleHash = Checksum256.Empty;
    public ProducerAuthoritySchedule Schedule = new();
};