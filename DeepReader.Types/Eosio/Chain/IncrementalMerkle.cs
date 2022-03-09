using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/incremental_merkle.hpp
/// </summary>
public class IncrementalMerkle
{
    public Checksum256[] ActiveNodes = Array.Empty<Checksum256>();
    public ulong NodeCount = 0;

    public static IncrementalMerkle ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new IncrementalMerkle();
        obj.ActiveNodes = new Checksum256[reader.ReadInt32()];
        for (int i = 0; i < obj.ActiveNodes.Length; i++)
        {
            obj.ActiveNodes[i] = reader.ReadString();
        }

        obj.NodeCount = reader.ReadUInt64();
        return obj;
    }
}