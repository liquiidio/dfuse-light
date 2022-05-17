namespace DeepReader.Types;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public class WaitWeight : IEosioSerializable<WaitWeight>
{
    public uint WaitSec = 0;//uint32
    public uint Weight = 0;//uint32

    public WaitWeight() { }

    public WaitWeight(BinaryReader reader)
    {
        WaitSec = reader.ReadUInt32();
        Weight = reader.ReadUInt32();
    }
    public static WaitWeight ReadFromBinaryReader(BinaryReader reader)
    {
        return new WaitWeight(reader);
    }
}