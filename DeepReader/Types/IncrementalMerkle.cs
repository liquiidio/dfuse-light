using DeepReader.EosTypes;

namespace DeepReader.Types;

public class IncrementalMerkle
{
    public Checksum256[] ActiveNodes = Array.Empty<Checksum256>();//[]Checksum256
    public ulong NodeCount = 0;//uint64
}