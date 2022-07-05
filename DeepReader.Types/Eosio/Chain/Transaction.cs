using System.Text.Json.Serialization;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.Extensions;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Interfaces;
using Action = DeepReader.Types.Eosio.Chain.Action;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public class Transaction : TransactionHeader, IEosioSerializable<Transaction>
{
    // abi-field-name: context_free_actions ,abi-field-type: action[]
    [JsonPropertyName("context_free_actions")]
    public Action[] ContextFreeActions;

    // abi-field-name: actions ,abi-field-type: action[]
    [JsonPropertyName("actions")]
    public Action[] Actions;

    // abi-field-name: transaction_extensions ,abi-field-type: extension[]
    [JsonPropertyName("transaction_extensions")]
    public Extension[] TransactionExtensions;

    public Transaction(IBufferReader reader) : base(reader)
    {
        ContextFreeActions = new Action[reader.Read7BitEncodedInt()];
        for (int i = 0; i < ContextFreeActions.Length; i++)
        {
            ContextFreeActions[i] = Action.ReadFromBinaryReader(reader);
        }

        Actions = new Action[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Actions.Length; i++)
        {
            Actions[i] = Action.ReadFromBinaryReader(reader);
        }

        TransactionExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i < TransactionExtensions.Length; i++)
        {
            TransactionExtensions[i] = reader.ReadExtension();
        }
    }

    public new static Transaction ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new Transaction(reader);
    }
}