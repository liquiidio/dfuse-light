using DeepReader.Types.Eosio.Chain.Legacy;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Detail;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public struct ScheduleInfo : IEosioSerializable<ScheduleInfo>
{
    public uint ScheduleLibNum = 0; /// last irr block num
    public Checksum256 ScheduleHash = Checksum256.Empty;
    public ProducerAuthoritySchedule Schedule = new();

    public ScheduleInfo()
    {

    }

    public static ScheduleInfo ReadFromBinaryReader(BinaryReader reader)
    {
        ScheduleInfo info = new ScheduleInfo();
        info.ScheduleLibNum = reader.ReadUInt32();
        info.ScheduleHash = reader.ReadChecksum256();
        info.Schedule = ProducerAuthoritySchedule.ReadFromBinaryReader(reader);
        return info;
    }
};