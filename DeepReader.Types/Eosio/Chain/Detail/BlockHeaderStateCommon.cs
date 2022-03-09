using DeepReader.Types.Helpers;
using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Types.Eosio.Chain.Detail;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public class BlockHeaderStateCommon
{
    [SortOrder(1)]
    public uint BlockNum = 0;
    [SortOrder(2)]
    public uint DPoSProposedIrreversibleBlockNum = 0;
    [SortOrder(3)]
    public uint DPoSIrreversibleBlockNum = 0;
    [SortOrder(4)]
    public ProducerAuthoritySchedule ActiveSchedule = new();
    [SortOrder(5)]
    public IncrementalMerkle BlockrootMerkle = new();
    [SortOrder(6)]
    public PairAccountNameBlockNum[] ProducerToLastProduced = Array.Empty<PairAccountNameBlockNum>();   //flat_map<account_name,uint32_t>
    [SortOrder(7)]
    public PairAccountNameBlockNum[] ProducerToLastImpliedIrb = Array.Empty<PairAccountNameBlockNum>(); // flat_map<account_name,uint32_t>
    [SortOrder(8)]
    public BlockSigningAuthorityVariant ValidBlockSigningAuthority = new BlockSigningAuthorityV0();
    [SortOrder(9)]
    public byte[] ConfirmCount = Array.Empty<byte>();

    public static BlockHeaderStateCommon ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new BlockHeaderStateCommon() 
        {
            BlockNum = reader.ReadUInt32(),
            DPoSProposedIrreversibleBlockNum = reader.ReadUInt32(),
            DPoSIrreversibleBlockNum = reader.ReadUInt32(),
            ActiveSchedule = ProducerAuthoritySchedule.ReadFromBinaryReader(reader),
            BlockrootMerkle = IncrementalMerkle.ReadFromBinaryReader(reader)
        };

        obj.ProducerToLastProduced = new PairAccountNameBlockNum[reader.ReadInt32()];
        for (int i = 0; i < obj.ProducerToLastProduced.Length; i++)
        {
            obj.ProducerToLastProduced[i] = PairAccountNameBlockNum.ReadFromBinaryReader(reader);
        }

        obj.ProducerToLastImpliedIrb = new PairAccountNameBlockNum[reader.ReadInt32()];
        for (int i = 0; i < obj.ProducerToLastImpliedIrb.Length; i++)
        {
            obj.ProducerToLastImpliedIrb[i] = PairAccountNameBlockNum.ReadFromBinaryReader(reader);
        }

        obj.ValidBlockSigningAuthority = BlockSigningAuthorityV0.ReadFromBinaryReader(reader);
        obj.ConfirmCount = reader.ReadBytes(reader.ReadInt32());

        return obj;
    }
}