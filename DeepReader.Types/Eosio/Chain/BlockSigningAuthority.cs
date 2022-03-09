namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// /// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public abstract class BlockSigningAuthorityVariant
{
}

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class BlockSigningAuthorityV0 : BlockSigningAuthorityVariant
{
    public uint Threshold = 0;
    public SharedKeyWeight[] Keys = Array.Empty<SharedKeyWeight>();

    public static BlockSigningAuthorityV0 ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new BlockSigningAuthorityV0()
        {
            Threshold = reader.ReadUInt32()
        };

        obj.Keys = new SharedKeyWeight[reader.ReadInt32()];
        for (int i = 0; i < obj.Keys.Length; i++)
        {
            obj.Keys[i] = SharedKeyWeight.ReadFromBinaryReader(reader);
        }
        return obj;
    }
}