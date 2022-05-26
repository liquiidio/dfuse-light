using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public class SignedTransaction : Transaction, IEosioSerializable<SignedTransaction>
{
    public Signature[] Signatures;

    public Bytes[] ContextFreeData; //< for each context-free action, there is an entry here

    public SignedTransaction(BinaryReader reader) : base(reader)
    {
        if (reader.BaseStream.Position == reader.BaseStream.Length) // Don't know exactly why but sometimes the stream is at it's end here already.
        {
            Signatures = Array.Empty<Signature>();
            ContextFreeData = Array.Empty<Bytes>();
            return;
        }

        Signatures = new Signature[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Signatures.Length; i++)
        {
            Signatures[i] = reader.ReadSignature();
        }

        ContextFreeData = new Bytes[reader.Read7BitEncodedInt()];
        for (int i = 0; i != ContextFreeData.Length; i++)
        {
            ContextFreeData[i] = reader.ReadBytes();
        }
    }

    public new static SignedTransaction ReadFromBinaryReader(BinaryReader reader)
    {
        return new SignedTransaction(reader);
    }
}