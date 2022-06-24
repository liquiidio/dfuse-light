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

    public IncrementalMerkle(BinaryBufferReader reader)
    {
        ActiveNodes = new Checksum256[reader.Read7BitEncodedInt()];
        for (int i = 0; i < ActiveNodes.Length; i++)
        {
            ActiveNodes[i] = Checksum256.ReadFromBinaryReader(reader);
        }

        NodeCount = reader.ReadUInt64();
    }
    public static IncrementalMerkle ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return new IncrementalMerkle(reader);
    }
}