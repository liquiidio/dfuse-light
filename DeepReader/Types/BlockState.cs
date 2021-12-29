using DeepReader.EosTypes;

namespace DeepReader.Types;

//public class BlockState : BlockHeaderState
//{

//}

//public class BlockHeaderState : BlockHeaderStateCommon
//{

//}

//public class BlockHeaderStateCommon
//{

//}

public class BlockState
{
    // From 'struct block_header_state_common'
    public uint BlockNum = 0;//uint32
    public uint DPoSProposedIrreversibleBlockNum = 0;//uint32
    public uint DPoSIrreversibleBlockNum = 0;//uint32
    public ProducerAuthoritySchedule ActiveSchedule = new ProducerAuthoritySchedule();//*eos.ProducerAuthoritySchedule
    public IncrementalMerkle BlockrootMerkle = new IncrementalMerkle();//*eos.MerkleRoot
    public PairAccountNameBlockNum[] ProducerToLastProduced = Array.Empty<PairAccountNameBlockNum>();//[]eos.PairAccountNameBlockNum
    public PairAccountNameBlockNum[] ProducerToLastImpliedIRB = Array.Empty<PairAccountNameBlockNum>();//[]eos.PairAccountNameBlockNum
    public BlockSigningAuthorityVariant ValidBlockSigningAuthority = new BlockSigningAuthorityV0();//*eos.BlockSigningAuthority
    public byte[] ConfirmCount = Array.Empty<byte>();//[]uint8

    // From 'struct block_header_state'
    public Checksum256 BlockID = string.Empty;//eos.Checksum256
    public SignedBlockHeader Header = new SignedBlockHeader();//*eos.SignedBlockHeader
    public PendingSchedule PendingSchedule = new PendingSchedule();//*eos.PendingSchedule
    public ProtocolFeatureActivationSet? ActivatedProtocolFeatures;//*eos.ProtocolFeatureActivationSet
    public Signature[] AdditionalSignatures = Array.Empty<Signature>();//[]ecc.Signature

    // From 'struct block_state'
    // Type changed in v2.1.x
    public SignedBlock? SignedBlock;//*SignedBlock
    public bool Validated = false;//bool
    
    // EOSIO 1.x
    //public byte[] BlockSigningKeyV1 = Array.Empty<byte>();//*ecc.PublicKey
}