namespace DeepReader.Types;

public class SignedBlockHeader : BlockHeader
{
    public byte[] ProducerSignature;// ecc.Signature // no pointer!!
}