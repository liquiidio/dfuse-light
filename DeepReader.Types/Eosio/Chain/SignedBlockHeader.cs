using DeepReader.Types.Helpers;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header.hpp
/// </summary>
public class SignedBlockHeader : BlockHeader, IEosioSerializable<SignedBlockHeader>
{
    [SortOrder(10)]
    public Signature ProducerSignature;// ecc.Signature // no pointer!!

    public SignedBlockHeader(BinaryReader reader) : base(reader)
    {
        ProducerSignature = reader.ReadSignature();
    }
    public new static SignedBlockHeader ReadFromBinaryReader(BinaryReader reader)
    {
        return new SignedBlockHeader(reader);
    }
}