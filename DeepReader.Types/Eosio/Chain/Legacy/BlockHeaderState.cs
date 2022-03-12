using DeepReader.Types.Helpers;
using DeepReader.Types.Eosio.Chain.Detail;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header_state.hpp
/// </summary>
public class BlockHeaderState : BlockHeaderStateCommon
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
        // Todo: (Haron) We might replace here once we confirm the cast does not work
        var blockHeaderState = (BlockHeaderState)BlockHeaderStateCommon.ReadFromBinaryReader(reader);
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