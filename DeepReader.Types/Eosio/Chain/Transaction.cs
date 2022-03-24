using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public class Transaction : TransactionHeader, IEosioSerializable<Transaction>
{
    // abi-field-name: context_free_actions ,abi-field-type: action[]
    [SortOrder(7)]
    [JsonPropertyName("context_free_actions")]
    public Action[] ContextFreeActions = Array.Empty<Action>();

    // abi-field-name: actions ,abi-field-type: action[]
    [SortOrder(8)]
    [JsonPropertyName("actions")]
    public Action[] Actions = Array.Empty<Action>();

    // abi-field-name: transaction_extensions ,abi-field-type: extension[]
    [SortOrder(9)]
    [JsonPropertyName("transaction_extensions")]
    public Extension[] TransactionExtensions = Array.Empty<Extension>();

    public Transaction()
    {
    }

    public Transaction(Action[] contextFreeActions, Action[] actions, Extension[] transactionExtensions)
    {
        this.ContextFreeActions = contextFreeActions;
        this.Actions = actions;
        this.TransactionExtensions = transactionExtensions;
    }

    public new static Transaction ReadFromBinaryReader(BinaryReader reader)
    {
        var transaction = new Transaction()
        {
            Expiration = reader.ReadTimestamp(),
            RefBlockNum = reader.ReadUInt16(),
            RefBlockPrefix = reader.ReadUInt32(),
            MaxNetUsageWords = reader.ReadVarUint32Obj(),
            MaxCpuUsageMs = reader.ReadByte(),
            DelaySec = reader.ReadVarUint32Obj()
        };

        transaction.ContextFreeActions = new Action[reader.Read7BitEncodedInt()];
        for (int i = 0; i < transaction.ContextFreeActions.Length; i++)
        {
            transaction.ContextFreeActions[i] = Action.ReadFromBinaryReader(reader);
        }

        transaction.Actions = new Action[reader.Read7BitEncodedInt()];
        for (int i = 0; i < transaction.Actions.Length; i++)
        {
            transaction.Actions[i] = Action.ReadFromBinaryReader(reader);
        }

        transaction.TransactionExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i < transaction.TransactionExtensions.Length; i++)
        {
            transaction.TransactionExtensions[i] = new Extension(reader.ReadUInt16(), reader.ReadChars(reader.Read7BitEncodedInt()));
        }

        return transaction;
    }
}