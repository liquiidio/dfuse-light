using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public sealed class SignedTransaction : Transaction, IEosioSerializable<SignedTransaction>
{
    public Signature[] Signatures;

    public Bytes[] ContextFreeData; //< for each context-free action, there is an entry here

    public SignedTransaction(BinaryBufferReader reader) : base(reader)
    {
        //if (reader.BaseStream.Position == reader.BaseStream.Length) // Don't know exactly why but sometimes the stream is at it's end here already.
        //{
           
        //}

        // I've replaced this with a try catch, I assume if the stream is at its end we will get an error which will be handled by creating empty arrays.

        try
        {
            Signatures = new Signature[reader.Read7BitEncodedInt()];
            for (int i = 0; i < Signatures.Length; i++)
            {
                Signatures[i] = Signature.ReadFromBinaryReader(reader);
            }

            ContextFreeData = new Bytes[reader.Read7BitEncodedInt()];
            for (int i = 0; i != ContextFreeData.Length; i++)
            {
                ContextFreeData[i] = Bytes.ReadFromBinaryReader(reader);
            }
        }
        catch
        {
            Signatures = Array.Empty<Signature>();
            ContextFreeData = Array.Empty<Bytes>();
            return;
        }
       
    }

    public new static SignedTransaction ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return new SignedTransaction(reader);
    }
}