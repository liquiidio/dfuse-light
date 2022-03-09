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

    // Todo: (Haron)
    public new static BlockHeaderState ReadFromBinaryReader(BinaryReader reader)
    {
        // Todo: @corvin does this work.
        // We call the static method the base class and cast it to this type and fill the remaining fields.
        var obj = (BlockHeaderState)BlockHeaderStateCommon.ReadFromBinaryReader(reader);
        obj.Id = reader.ReadString();
        obj.Header = SignedBlockHeader.ReadFromBinaryReader(reader);
        obj.PendingSchedule = ScheduleInfo.ReadFromBinaryReader(reader);
        obj.ActivatedProtocolFeatures = ProtocolFeatureActivationSet.ReadFromBinaryReader(reader);

        obj.AdditionalSignatures = new Signature[reader.ReadInt32()];
        for (int i = 0; i < obj.AdditionalSignatures.Length; i++)
        {
            obj.AdditionalSignatures[i] = reader.ReadString();
        }

        return obj;
    }
}