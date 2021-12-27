namespace DeepReader.Types;

public class BlockState
{
    public uint BlockNum = 0;//uint32
    public uint DPoSProposedIrreversibleBlockNum = 0;//uint32
    public uint DPoSIrreversibleBlockNum = 0;//uint32
    public ProducerScheduleOrAuthoritySchedule ActiveSchedule = new ProducerScheduleOrAuthoritySchedule();//*eos.ProducerAuthoritySchedule
    public MerkleRoot BlockrootMerkle = new MerkleRoot();//*eos.MerkleRoot
    public PairAccountNameBlockNum[] ProducerToLastProduced = Array.Empty<PairAccountNameBlockNum>();//[]eos.PairAccountNameBlockNum
    public PairAccountNameBlockNum[] ProducerToLastImpliedIRB = Array.Empty<PairAccountNameBlockNum>();//[]eos.PairAccountNameBlockNum
    public BlockSigningAuthority ValidBlockSigningAuthorityV2 = new BlockSigningAuthority();//*eos.BlockSigningAuthority
    public byte[] ConfirmCount = Array.Empty<byte>();//[]uint8

    // From 'struct block_header_state'
    public byte[] BlockID = Array.Empty<byte>();//eos.Checksum256
    public SignedBlockHeader Header = new SignedBlockHeader();//*eos.SignedBlockHeader
    public PendingSchedule PendingSchedule = new PendingSchedule();//*eos.PendingSchedule
    public ProtocolFeatureActivationSet ActivatedProtocolFeatures = new ProtocolFeatureActivationSet();//*eos.ProtocolFeatureActivationSet
    public byte[] AdditionalSignatures = Array.Empty<byte>();//[]ecc.Signature

    // From 'struct block_state'
    // Type changed in v2.1.x
    public SignedBlock SignedBlock = new SignedBlock();//*SignedBlock
    public bool Validated = false;//bool
    
    // EOSIO 1.x
    //public byte[] BlockSigningKeyV1 = Array.Empty<byte>();//*ecc.PublicKey
}