using DeepReader.Types.Helpers;
using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Types.Eosio.Chain.Detail;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public class BlockHeaderStateCommon
{
    [SortOrder(1)]
    public uint BlockNum;
    [SortOrder(2)]
    public uint DPoSProposedIrreversibleBlockNum;
    [SortOrder(3)]
    public uint DPoSIrreversibleBlockNum;
    [SortOrder(4)]
    public ProducerAuthoritySchedule ActiveSchedule;
    [SortOrder(5)]
    public IncrementalMerkle BlockrootMerkle;
    [SortOrder(6)]
    public PairAccountNameBlockNum[] ProducerToLastProduced = Array.Empty<PairAccountNameBlockNum>();   //flat_map<account_name,uint32_t>
    [SortOrder(7)]
    public PairAccountNameBlockNum[] ProducerToLastImpliedIrb = Array.Empty<PairAccountNameBlockNum>(); // flat_map<account_name,uint32_t>
    [SortOrder(8)]
    public BlockSigningAuthorityVariant ValidBlockSigningAuthority;
    [SortOrder(9)]
    public byte[] ConfirmCount = Array.Empty<byte>();

    public static BlockHeaderStateCommon ReadFromBinaryReader(BinaryReader reader)
    {
        var blockStateHeaderCommon = new BlockHeaderStateCommon()
        {
            BlockNum = reader.ReadUInt32(),
            DPoSProposedIrreversibleBlockNum = reader.ReadUInt32(),
            DPoSIrreversibleBlockNum = reader.ReadUInt32(),
            ActiveSchedule = ProducerAuthoritySchedule.ReadFromBinaryReader(reader),
            BlockrootMerkle = IncrementalMerkle.ReadFromBinaryReader(reader)
        };

        blockStateHeaderCommon.ProducerToLastProduced = new PairAccountNameBlockNum[reader.Read7BitEncodedInt()];
        for (int i = 0; i < blockStateHeaderCommon.ProducerToLastProduced.Length; i++)
        {
            blockStateHeaderCommon.ProducerToLastProduced[i] = PairAccountNameBlockNum.ReadFromBinaryReader(reader);
        }

        blockStateHeaderCommon.ProducerToLastImpliedIrb = new PairAccountNameBlockNum[reader.Read7BitEncodedInt()];
        for (int i = 0; i < blockStateHeaderCommon.ProducerToLastImpliedIrb.Length; i++)
        {
            blockStateHeaderCommon.ProducerToLastImpliedIrb[i] = PairAccountNameBlockNum.ReadFromBinaryReader(reader);
        }

        blockStateHeaderCommon.ValidBlockSigningAuthority = BlockSigningAuthorityVariant.ReadFromBinaryReader(reader);

        blockStateHeaderCommon.ConfirmCount = reader.ReadBytes(reader.Read7BitEncodedInt());

        return blockStateHeaderCommon;
    }
}