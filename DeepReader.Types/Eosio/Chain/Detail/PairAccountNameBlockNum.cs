using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Detail;

/// <summary>
/// replaces key and value of flat_map<account_name, uint32_t>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public class PairAccountNameBlockNum : IEosioSerializable<PairAccountNameBlockNum>
{
    public Name AccountName = string.Empty;
    public uint BlockNum = 0;

    public static PairAccountNameBlockNum ReadFromBinaryReader(BinaryReader reader)
    {
        var pairAccountNameBlockNum = new PairAccountNameBlockNum()
        {
            AccountName = reader.ReadName(),
            BlockNum = reader.ReadUInt32()
        };
        return pairAccountNameBlockNum;
    }
}