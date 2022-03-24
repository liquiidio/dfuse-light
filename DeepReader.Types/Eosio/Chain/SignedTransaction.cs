using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public class SignedTransaction : Transaction, IEosioSerializable<SignedTransaction>
{
    [SortOrder(10)]
    public Signature[] Signatures = Array.Empty<Signature>();

    [SortOrder(11)]
    public Bytes[] ContextFreeData = Array.Empty<Bytes>(); //< for each context-free action, there is an entry here

    public new static SignedTransaction ReadFromBinaryReader(BinaryReader reader)
    {
        var signedTransaction = new SignedTransaction()
        {
            Expiration = reader.ReadTimestamp(),
            RefBlockNum = reader.ReadUInt16(),
            RefBlockPrefix = reader.ReadUInt32(),
            MaxNetUsageWords = reader.ReadVarUint32Obj(),
            MaxCpuUsageMs = reader.ReadByte(),
            DelaySec = reader.ReadVarUint32Obj()
        };

        signedTransaction.ContextFreeActions = new Action[reader.Read7BitEncodedInt()];
        for (int i = 0; i < signedTransaction.ContextFreeActions.Length; i++)
        {
            signedTransaction.ContextFreeActions[i] = Action.ReadFromBinaryReader(reader);
        }

        signedTransaction.Actions = new Action[reader.Read7BitEncodedInt()];
        for (int i = 0; i < signedTransaction.Actions.Length; i++)
        {
            signedTransaction.Actions[i] = Action.ReadFromBinaryReader(reader);
        }

        signedTransaction.TransactionExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i < signedTransaction.TransactionExtensions.Length; i++)
        {
            signedTransaction.TransactionExtensions[i] = new Extension(reader.ReadUInt16(), reader.ReadChars(reader.Read7BitEncodedInt()));
        }

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