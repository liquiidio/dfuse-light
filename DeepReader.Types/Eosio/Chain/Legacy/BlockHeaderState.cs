using DeepReader.Types.Helpers;
using DeepReader.Types.Eosio.Chain.Detail;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public class BlockHeaderState : BlockHeaderStateCommon, IEosioSerializable<BlockHeaderState>
{
    [SortOrder(10)]
    public Checksum256 Id;
    [SortOrder(11)]
    public SignedBlockHeader Header;
    [SortOrder(12)]
    public ScheduleInfo PendingSchedule;
    [SortOrder(13)]
    public ProtocolFeatureActivationSet? ActivatedProtocolFeatures;
    [SortOrder(14)]
    public Signature[] AdditionalSignatures;

    public BlockHeaderState(BinaryReader reader)
    {
        BlockNum = reader.ReadUInt32();
        DPoSProposedIrreversibleBlockNum = reader.ReadUInt32();
        DPoSIrreversibleBlockNum = reader.ReadUInt32();
        ActiveSchedule = ProducerAuthoritySchedule.ReadFromBinaryReader(reader);
        BlockrootMerkle = IncrementalMerkle.ReadFromBinaryReader(reader);

        ProducerToLastProduced = new PairAccountNameBlockNum[reader.Read7BitEncodedInt()];
        for (var i = 0; i < ProducerToLastProduced.Length; i++)
        {
            ProducerToLastProduced[i] = PairAccountNameBlockNum.ReadFromBinaryReader(reader);
        }

        ProducerToLastImpliedIrb = new PairAccountNameBlockNum[reader.Read7BitEncodedInt()];
        for (var i = 0; i < ProducerToLastImpliedIrb.Length; i++)
        {
            ProducerToLastImpliedIrb[i] = PairAccountNameBlockNum.ReadFromBinaryReader(reader);
        }

        ValidBlockSigningAuthority = BlockSigningAuthorityVariant.ReadFromBinaryReader(reader);

        ConfirmCount = reader.ReadBytes(reader.Read7BitEncodedInt());

        Id = reader.ReadChecksum256();
        Header = SignedBlockHeader.ReadFromBinaryReader(reader);
        PendingSchedule = ScheduleInfo.ReadFromBinaryReader(reader);

        var readActivatedProtocolFeatures = reader.ReadBoolean();

        if (readActivatedProtocolFeatures)
            ActivatedProtocolFeatures = ProtocolFeatureActivationSet.ReadFromBinaryReader(reader);

        AdditionalSignatures = new Signature[reader.Read7BitEncodedInt()];
        for (var i = 0; i < AdditionalSignatures.Length; i++)
        {
            AdditionalSignatures[i] = reader.ReadSignature();
        }
    }

    public new static BlockHeaderState ReadFromBinaryReader(BinaryReader reader)
    {
        return new BlockHeaderState(reader);
    }
}