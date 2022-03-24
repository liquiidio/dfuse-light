using DeepReader.Types.Helpers;
using DeepReader.Types.Eosio.Chain.Detail;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public class BlockHeaderState : BlockHeaderStateCommon, IEosioSerializable<BlockHeaderState>
{
    [SortOrder(10)]
    public Checksum256 Id = Checksum256.Empty;
    [SortOrder(11)]
    public SignedBlockHeader Header = new();
    [SortOrder(12)]
    public ScheduleInfo PendingSchedule = new();
    [SortOrder(13)]
    public ProtocolFeatureActivationSet? ActivatedProtocolFeatures;
    [SortOrder(14)]
    public Signature[] AdditionalSignatures = Array.Empty<Signature>();

    public new static BlockHeaderState ReadFromBinaryReader(BinaryReader reader)
    {
        var blockHeaderState = new BlockHeaderState()
        {
            BlockNum = reader.ReadUInt32(),
            DPoSProposedIrreversibleBlockNum = reader.ReadUInt32(),
            DPoSIrreversibleBlockNum = reader.ReadUInt32(),
            ActiveSchedule = ProducerAuthoritySchedule.ReadFromBinaryReader(reader),
            BlockrootMerkle = IncrementalMerkle.ReadFromBinaryReader(reader)
        };

        blockHeaderState.ProducerToLastProduced = new PairAccountNameBlockNum[reader.Read7BitEncodedInt()];
        for (int i = 0; i < blockHeaderState.ProducerToLastProduced.Length; i++)
        {
            blockHeaderState.ProducerToLastProduced[i] = PairAccountNameBlockNum.ReadFromBinaryReader(reader);
        }

        blockHeaderState.ProducerToLastImpliedIrb = new PairAccountNameBlockNum[reader.Read7BitEncodedInt()];
        for (int i = 0; i < blockHeaderState.ProducerToLastImpliedIrb.Length; i++)
        {
            blockHeaderState.ProducerToLastImpliedIrb[i] = PairAccountNameBlockNum.ReadFromBinaryReader(reader);
        }

        blockHeaderState.ValidBlockSigningAuthority = BlockSigningAuthorityVariant.ReadFromBinaryReader(reader);

        blockHeaderState.ConfirmCount = reader.ReadBytes(reader.Read7BitEncodedInt());
        
        blockHeaderState.Id = reader.ReadChecksum256();
        blockHeaderState.Header = SignedBlockHeader.ReadFromBinaryReader(reader);
        blockHeaderState.PendingSchedule = ScheduleInfo.ReadFromBinaryReader(reader);

        var readActivatedProtocolFeatures = reader.ReadBoolean();

        if (readActivatedProtocolFeatures)
            blockHeaderState.ActivatedProtocolFeatures = ProtocolFeatureActivationSet.ReadFromBinaryReader(reader);

        blockHeaderState.AdditionalSignatures = new Signature[reader.Read7BitEncodedInt()];
        for (int i = 0; i < blockHeaderState.AdditionalSignatures.Length; i++)
        {
            blockHeaderState.AdditionalSignatures[i] = reader.ReadSignature();
        }

        return blockHeaderState;
    }
}