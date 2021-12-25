namespace DeepReader.Types;

public class BlockState
{
    public uint BlockNum;//                         uint32                         `json:"block_num"`
    public uint DPoSProposedIrreversibleBlockNum;// uint32                         `json:"dpos_proposed_irreversible_blocknum"`
    public uint DPoSIrreversibleBlockNum;//         uint32                         `json:"dpos_irreversible_blocknum"`
    public ProducerScheduleOrAuthoritySchedule ActiveSchedule;//                   *eos.ProducerAuthoritySchedule `json:"active_schedule"`
    public MerkleRoot BlockrootMerkle;//                  *eos.MerkleRoot                `json:"blockroot_merkle,omitempty"`
    public PairAccountNameBlockNum[] ProducerToLastProduced;//           []eos.PairAccountNameBlockNum  `json:"producer_to_last_produced,omitempty"`
    public PairAccountNameBlockNum[] ProducerToLastImpliedIRB;//         []eos.PairAccountNameBlockNum  `json:"producer_to_last_implied_irb,omitempty"`
    public BlockSigningAuthority ValidBlockSigningAuthorityV2;//     *eos.BlockSigningAuthority     `json:"valid_block_signing_authority,omitempty"`
    public byte[] ConfirmCount;//                     []uint8                        `json:"confirm_count,omitempty"`

    // From 'struct block_header_state'
    public byte[] BlockID { get; set; } = Array.Empty<byte>();//                   eos.Checksum256                   `json:"id"`
    public SignedBlockHeader Header;//                    *eos.SignedBlockHeader            `json:"header,omitempty"`
    public PendingSchedule PendingSchedule;//           *eos.PendingSchedule              `json:"pending_schedule"`
    public ProtocolFeatureActivationSet ActivatedProtocolFeatures;// *eos.ProtocolFeatureActivationSet `json:"activated_protocol_features,omitempty" eos:"optional"`
    public byte[] AdditionalSignatures;//      []ecc.Signature                   `json:"additional_signatures"`

    // From 'struct block_state'
    // Type changed in v2.1.x
    public SignedBlock SignedBlock;// *SignedBlock `json:"block,omitempty" eos:"optional"`
    public bool Validated;//   bool         `json:"validated"`
    
    // EOSIO 1.x
    public byte[] BlockSigningKeyV1;// *ecc.PublicKey `json:"block_signing_key,omitempty" eos:"-"`

}