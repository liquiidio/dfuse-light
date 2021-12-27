namespace DeepReader.Types;

public class Authority
{
    public uint Threshold = 0;//uint32
    public KeyWeight[] Keys = Array.Empty<KeyWeight>();//[]*KeyWeight
    public PermissionLevelWeight[] Accounts = Array.Empty<PermissionLevelWeight>();//[]*PermissionLevelWeight
    public WaitWeight[] Waits = Array.Empty<WaitWeight>();//[]*WaitWeight
}