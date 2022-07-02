using DeepReader.Types.Eosio.Chain.Detail;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;
using DeepReader.Types.Eosio.Chain.Legacy;
using DeepReader.Types.Extensions;
using DeepReader.Types.Infrastructure.BinaryReaders;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public class BlockHeaderState : BlockHeaderStateCommon, IEosioSerializable<BlockHeaderState>
{
    public Checksum256 Id;

    public SignedBlockHeader Header;

    public ScheduleInfo PendingSchedule;

    public ProtocolFeatureActivationSet? ActivatedProtocolFeatures;

    public Signature[] AdditionalSignatures;

    public BlockHeaderState(IBufferReader reader)
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

        Id = Checksum256.ReadFromBinaryReader(reader);
        Header = SignedBlockHeader.ReadFromBinaryReader(reader);
        PendingSchedule = ScheduleInfo.ReadFromBinaryReader(reader);

        var readActivatedProtocolFeatures = reader.ReadBoolean();

        if (readActivatedProtocolFeatures)
            ActivatedProtocolFeatures = ProtocolFeatureActivationSet.ReadFromBinaryReader(reader);

        AdditionalSignatures = new Signature[reader.Read7BitEncodedInt()];
        for (var i = 0; i < AdditionalSignatures.Length; i++)
        {
            AdditionalSignatures[i] = Signature.ReadFromBinaryReader(reader);
        }
    }

    public new static BlockHeaderState ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new BlockHeaderState(reader);
    }
}