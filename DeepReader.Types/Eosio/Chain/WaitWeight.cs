namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public sealed class WaitWeight : IEosioSerializable<WaitWeight>
{
    public uint WaitSec = 0;//uint32
    public WeightType Weight = 0;//uint16

    public WaitWeight() { }

    public WaitWeight(BinaryBufferReader reader)
    {
        WaitSec = reader.ReadUInt32();
        Weight = reader.ReadUInt16();
    }
    public static WaitWeight ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return new WaitWeight(reader);
    }
}