using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public class SignedTransaction : Transaction
{
    [SortOrder(10)]
    public Signature[] Signatures = Array.Empty<Signature>();

    [SortOrder(11)]
    public Bytes[] ContextFreeData = Array.Empty<Bytes>(); //< for each context-free action, there is an entry here

    public new static SignedTransaction ReadFromBinaryReader(BinaryReader reader)
    {
        // Todo: (Haron) Type cast is needed to be added here once confirmed
        var signedTransaction = new SignedTransaction();

        signedTransaction.Signatures = new Signature[reader.Read7BitEncodedInt()];
        for (int i = 0; i < signedTransaction.Signatures.Length; i++)
        {
            signedTransaction.Signatures[i] = reader.ReadSignature();
        }

        signedTransaction.ContextFreeData = new Bytes[reader.Read7BitEncodedInt()];
        for (int i = 0; i != signedTransaction.ContextFreeData.Length; i++)
        {
            signedTransaction.ContextFreeData[i] = reader.ReadBytes();
        }
        return signedTransaction;
    }
}