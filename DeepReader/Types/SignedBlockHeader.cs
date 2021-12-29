using DeepReader.EosTypes;

namespace DeepReader.Types;

public class SignedBlockHeader : BlockHeader
{
    [SortOrder(10)]
    public Signature ProducerSignature = string.Empty;// ecc.Signature // no pointer!!
}