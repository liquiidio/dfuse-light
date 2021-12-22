using DeepReader.Types;

namespace DeepReader;

public class SignedBlockHeader : BlockHeader
{
    public byte[] ProducerSignature;// ecc.Signature // no pointer!!
}