using System.Text.Json.Serialization;

namespace DeepReader.Types;

[Serializable()]
public class Transaction : TransactionHeader
{
    // abi-field-name: context_free_actions ,abi-field-type: action[]
    [SortOrder(7)]
    [JsonPropertyName("context_free_actions")]
    public Action[] ContextFreeActions;

    // abi-field-name: actions ,abi-field-type: action[]
    [SortOrder(8)]
    [JsonPropertyName("actions")]
    public Action[] Actions;

    // abi-field-name: transaction_extensions ,abi-field-type: extension[]
    [SortOrder(9)]
    [JsonPropertyName("transaction_extensions")]
    public Extension[] TransactionExtensions;

    public Transaction(Action[] contextFreeActions, Action[] actions, Extension[] transactionExtensions)
    {
        this.ContextFreeActions = contextFreeActions;
        this.Actions = actions;
        this.TransactionExtensions = transactionExtensions;
    }

    public Transaction()
    {
    }
}