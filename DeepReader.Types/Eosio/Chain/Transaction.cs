using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public class Transaction : TransactionHeader
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
}