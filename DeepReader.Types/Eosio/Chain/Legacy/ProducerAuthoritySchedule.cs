using DeepReader.Types.Extensions;
using Salar.BinaryBuffers;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public sealed class ProducerAuthoritySchedule : IEosioSerializable<ProducerAuthoritySchedule>
{
    public uint Version;
    public ProducerAuthority[] Producers;

    public ProducerAuthoritySchedule(BinaryBufferReader reader)
    {
        Version = reader.ReadUInt32();

        Producers = new ProducerAuthority[reader.Read7BitEncodedInt()];
        for (var i = 0; i < Producers.Length; i++)
        {
            Producers[i] = ProducerAuthority.ReadFromBinaryReader(reader);
        }
    }

    public static ProducerAuthoritySchedule ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return new ProducerAuthoritySchedule(reader);
    }
}