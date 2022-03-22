using DeepReader.Types.Helpers;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header.hpp
/// </summary>
public class SignedBlockHeader : BlockHeader, IEosioSerializable<SignedBlockHeader>
{
    [SortOrder(10)]
    public Signature ProducerSignature = Signature.Empty;// ecc.Signature // no pointer!!

    public new static SignedBlockHeader ReadFromBinaryReader(BinaryReader reader)
    {
        var signedBloackHeader = (SignedBlockHeader)BlockHeader.ReadFromBinaryReader(reader);
        signedBloackHeader.ProducerSignature = reader.ReadSignature();
        return signedBloackHeader;
    }
}