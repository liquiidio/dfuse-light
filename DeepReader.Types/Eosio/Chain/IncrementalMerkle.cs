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
        // Todo(Haron): IcrementalMerkle - finish writing this method
        return new();
    }
}