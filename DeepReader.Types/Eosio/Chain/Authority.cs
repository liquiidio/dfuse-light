namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public class Authority : IEosioSerializable<Authority>
{
    public uint Threshold = 0;//uint32
    public KeyWeight[] Keys = Array.Empty<KeyWeight>();//[]*KeyWeight
    public PermissionLevelWeight[] Accounts = Array.Empty<PermissionLevelWeight>();//[]*PermissionLevelWeight
    public WaitWeight[] Waits = Array.Empty<WaitWeight>();//[]*WaitWeight

    public Authority() { }

    public Authority(BinaryReader reader)
    {
        Threshold = reader.ReadUInt32();

        Keys = new KeyWeight[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Keys.Length; i++)
        {
            Keys[i] = KeyWeight.ReadFromBinaryReader(reader);
        }

        Accounts = new PermissionLevelWeight[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Accounts.Length; i++)
        {
            Accounts[i] = PermissionLevelWeight.ReadFromBinaryReader(reader);
        }

        Waits = new WaitWeight[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Waits.Length; i++)
        {
            Waits[i] = WaitWeight.ReadFromBinaryReader(reader);
        }
    }

    public static Authority ReadFromBinaryReader(BinaryReader reader)
    {
        return new Authority(reader);
    }
}