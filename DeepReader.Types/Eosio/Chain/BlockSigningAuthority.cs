namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// /// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public abstract class BlockSigningAuthorityVariant : IEosioSerializable<BlockSigningAuthorityVariant>
{
    public static BlockSigningAuthorityVariant ReadFromBinaryReader(BinaryReader reader)
    {
        var type = reader.ReadByte();
        switch (type)
        {
            case 0:
                return BlockSigningAuthorityV0.ReadFromBinaryReader(reader);
            default:
                throw new Exception("BlockSigningAuthorityVariant {type} unknown");
        }
    }
}

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class BlockSigningAuthorityV0 : BlockSigningAuthorityVariant, IEosioSerializable<BlockSigningAuthorityV0>
{
    public uint Threshold = 0;
    public SharedKeyWeight[] Keys = Array.Empty<SharedKeyWeight>();

    public new static BlockSigningAuthorityV0 ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new BlockSigningAuthorityV0()
        {
            Threshold = reader.ReadUInt32()
        };

        obj.Keys = new SharedKeyWeight[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.Keys.Length; i++)
        {
            obj.Keys[i] = SharedKeyWeight.ReadFromBinaryReader(reader);
        }
        return obj;
    }
}