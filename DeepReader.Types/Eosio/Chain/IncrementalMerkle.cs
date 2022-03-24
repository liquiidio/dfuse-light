using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/incremental_merkle.hpp
/// </summary>
public class IncrementalMerkle : IEosioSerializable<IncrementalMerkle>
{
    public Checksum256[] ActiveNodes = Array.Empty<Checksum256>();
    public ulong NodeCount = 0;

    public static IncrementalMerkle ReadFromBinaryReader(BinaryReader reader)
    {
        var incrementalMerkle = new IncrementalMerkle();
        incrementalMerkle.ActiveNodes = new Checksum256[reader.Read7BitEncodedInt()];
        for (int i = 0; i < incrementalMerkle.ActiveNodes.Length; i++)
        {
            incrementalMerkle.ActiveNodes[i] = reader.ReadChecksum256();
        }

        incrementalMerkle.NodeCount = reader.ReadUInt64();
        return incrementalMerkle;
    }
}