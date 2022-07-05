using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Interfaces;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/incremental_merkle.hpp
/// </summary>
public sealed class IncrementalMerkle : IEosioSerializable<IncrementalMerkle>
{
    public Checksum256[] ActiveNodes;
    public ulong NodeCount;

    public IncrementalMerkle(IBufferReader reader)
    {
        ActiveNodes = new Checksum256[reader.Read7BitEncodedInt()];
        for (int i = 0; i < ActiveNodes.Length; i++)
        {
            ActiveNodes[i] = Checksum256.ReadFromBinaryReader(reader);
        }

        NodeCount = reader.ReadUInt64();
    }
    public static IncrementalMerkle ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new IncrementalMerkle(reader);
    }
}