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
}