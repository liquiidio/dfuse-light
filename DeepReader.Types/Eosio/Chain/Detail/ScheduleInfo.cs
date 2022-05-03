using DeepReader.Types.Eosio.Chain.Legacy;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain.Detail;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public struct ScheduleInfo : IEosioSerializable<ScheduleInfo>
{
    public uint ScheduleLibNum; /// last irr block num
    public Checksum256 ScheduleHash;
    public ProducerAuthoritySchedule Schedule;

    public ScheduleInfo(BinaryReader reader)
    {
        ScheduleLibNum = reader.ReadUInt32();
        ScheduleHash = reader.ReadChecksum256();
        Schedule = ProducerAuthoritySchedule.ReadFromBinaryReader(reader);
    }

    public static ScheduleInfo ReadFromBinaryReader(BinaryReader reader)
    {
        return new ScheduleInfo(reader);
    }
};