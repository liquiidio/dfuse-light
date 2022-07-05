using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Interfaces;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public sealed class ProducerAuthority : IEosioSerializable<ProducerAuthority>
{
    public Name AccountName;
    public BlockSigningAuthorityVariant BlockSigningAuthority;

    public ProducerAuthority(IBufferReader reader)
    {
        AccountName = Name.ReadFromBinaryReader(reader);
        BlockSigningAuthority = BlockSigningAuthorityVariant.ReadFromBinaryReader(reader);
    }
    public static ProducerAuthority ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new ProducerAuthority(reader);
    }
}