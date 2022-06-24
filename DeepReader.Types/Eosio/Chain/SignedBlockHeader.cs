using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header.hpp
/// </summary>
public class SignedBlockHeader : BlockHeader, IEosioSerializable<SignedBlockHeader>
{
    public Signature ProducerSignature;// ecc.Signature // no pointer!!

    public SignedBlockHeader(BinaryBufferReader reader) : base(reader)
    {
        ProducerSignature = Signature.ReadFromBinaryReader(reader);
    }
    public new static SignedBlockHeader ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return new SignedBlockHeader(reader);
    }
}