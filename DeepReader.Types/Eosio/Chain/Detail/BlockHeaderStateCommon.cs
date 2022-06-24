using DeepReader.Types.Eosio.Chain.Legacy;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain.Detail;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public class BlockHeaderStateCommon : IEosioSerializable<BlockHeaderStateCommon>, IFasterSerializable<BlockHeaderStateCommon>
{
    public uint BlockNum;

    public uint DPoSProposedIrreversibleBlockNum;

    public uint DPoSIrreversibleBlockNum;

    public ProducerAuthoritySchedule ActiveSchedule;

    public IncrementalMerkle BlockrootMerkle;

    public PairAccountNameBlockNum[] ProducerToLastProduced = Array.Empty<PairAccountNameBlockNum>();   //flat_map<account_name,uint32_t>

    public PairAccountNameBlockNum[] ProducerToLastImpliedIrb = Array.Empty<PairAccountNameBlockNum>(); // flat_map<account_name,uint32_t>

    public BlockSigningAuthorityVariant ValidBlockSigningAuthority;

    public byte[] ConfirmCount = Array.Empty<byte>();

    public static BlockHeaderStateCommon ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
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

    public static BlockHeaderStateCommon ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        throw new NotImplementedException();
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}