namespace DeepReader.Types;

public class SignedBlockHeader : BlockHeader
{
    public byte[] ProducerSignature = Array.Empty<byte>();// ecc.Signature // no pointer!!
}