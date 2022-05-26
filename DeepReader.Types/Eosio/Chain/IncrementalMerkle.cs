using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/incremental_merkle.hpp
/// </summary>
public sealed class IncrementalMerkle : IEosioSerializable<IncrementalMerkle>
{
    public Checksum256[] ActiveNodes;
    public ulong NodeCount;

    public IncrementalMerkle(BinaryReader reader)
    {
        ActiveNodes = new Checksum256[reader.Read7BitEncodedInt()];
        for (int i = 0; i < ActiveNodes.Length; i++)
        {
            ActiveNodes[i] = reader.ReadChecksum256();
        }

        NodeCount = reader.ReadUInt64();
    }
    public static IncrementalMerkle ReadFromBinaryReader(BinaryReader reader)
    {
        return new IncrementalMerkle(reader);
    }
}