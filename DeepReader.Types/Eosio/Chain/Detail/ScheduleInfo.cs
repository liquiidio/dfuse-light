using DeepReader.Types.Eosio.Chain.Legacy;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure.BinaryReaders;

namespace DeepReader.Types.Eosio.Chain.Detail;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public struct ScheduleInfo : IEosioSerializable<ScheduleInfo>
{
    public uint ScheduleLibNum; /// last irr block num
    public Checksum256 ScheduleHash;
    public ProducerAuthoritySchedule Schedule;

    public ScheduleInfo(IBufferReader reader)
    {
        ScheduleLibNum = reader.ReadUInt32();
        ScheduleHash = Checksum256.ReadFromBinaryReader(reader);
        Schedule = ProducerAuthoritySchedule.ReadFromBinaryReader(reader);
    }

    public static ScheduleInfo ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new ScheduleInfo(reader);
    }
};