using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Detail;

/// <summary>
/// replaces key and value of flat_map<account_name, uint32_t>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public sealed class PairAccountNameBlockNum : IEosioSerializable<PairAccountNameBlockNum>
{
    public Name AccountName;
    public uint BlockNum;

    public PairAccountNameBlockNum(BinaryReader reader)
    {
        AccountName = Name.ReadFromBinaryReader(reader);
        BlockNum = reader.ReadUInt32();
    }
    public static PairAccountNameBlockNum ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        return new PairAccountNameBlockNum(reader);
    }
}