using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Types;

public sealed class PermissionObject
{
    // UsageId represents the EOSIO internal id of this permission object.
    public ulong UsageId = 0;//uint64
    // Parent represents the EOSIO internal id of the parent's of this permission object.
    public ulong Parent = 0;//uint64
    // Owner is the account for which this permission was set
    public string Owner = string.Empty;//string
    // Name is the permission's name this permission object is known as.
    public string Name = string.Empty;//string
    public DateTime LastUpdated = new();//*timestamp.Timestamp
    public Authority Auth = new();//*Authority

    public PermissionObject() { }
}