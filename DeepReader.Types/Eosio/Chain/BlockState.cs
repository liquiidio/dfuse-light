using DeepReader.Types.Eosio.Chain.Detail;
using DeepReader.Types.Helpers;
using DeepReader.Types.Eosio.Chain.Legacy;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_state.hpp
/// </summary>
public class BlockState : BlockHeaderState, IEosioSerializable<BlockState>
{
    [SortOrder(15)]
    public SignedBlock? Block;
    [SortOrder(16)]
    public bool Validated = false;

    public new static BlockState ReadFromBinaryReader(BinaryReader reader)
    {
        var blockState = new BlockState()
        {
            BlockNum = reader.ReadUInt32(),
            DPoSProposedIrreversibleBlockNum = reader.ReadUInt32(),
            DPoSIrreversibleBlockNum = reader.ReadUInt32(),
            ActiveSchedule = ProducerAuthoritySchedule.ReadFromBinaryReader(reader),
            BlockrootMerkle = IncrementalMerkle.ReadFromBinaryReader(reader)
        };

        blockState.ProducerToLastProduced = new PairAccountNameBlockNum[reader.Read7BitEncodedInt()];
        for (int i = 0; i < blockState.ProducerToLastProduced.Length; i++)
        {
            blockState.ProducerToLastProduced[i] = PairAccountNameBlockNum.ReadFromBinaryReader(reader);
        }

        blockState.ProducerToLastImpliedIrb = new PairAccountNameBlockNum[reader.Read7BitEncodedInt()];
        for (int i = 0; i < blockState.ProducerToLastImpliedIrb.Length; i++)
        {
            blockState.ProducerToLastImpliedIrb[i] = PairAccountNameBlockNum.ReadFromBinaryReader(reader);
        }

        blockState.ValidBlockSigningAuthority = BlockSigningAuthorityVariant.ReadFromBinaryReader(reader);

        blockState.ConfirmCount = reader.ReadBytes(reader.Read7BitEncodedInt());

        blockState.Id = reader.ReadChecksum256();
        blockState.Header = SignedBlockHeader.ReadFromBinaryReader(reader);
        blockState.PendingSchedule = ScheduleInfo.ReadFromBinaryReader(reader);

        var readActivatedProtocolFeatures = reader.ReadBoolean();

        if (readActivatedProtocolFeatures)
            blockState.ActivatedProtocolFeatures = ProtocolFeatureActivationSet.ReadFromBinaryReader(reader);

        blockState.AdditionalSignatures = new Signature[reader.Read7BitEncodedInt()];
        for (int i = 0; i < blockState.AdditionalSignatures.Length; i++)
        {
            blockState.AdditionalSignatures[i] = reader.ReadSignature();
        }
        var readBlock = reader.ReadBoolean();

        if (readBlock)
            blockState.Block = SignedBlock.ReadFromBinaryReader(reader);
        
        blockState.Validated = reader.ReadBoolean();
        return blockState;
    }
}