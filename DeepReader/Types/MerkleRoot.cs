namespace DeepReader.Types;

public class MerkleRoot
{
    public byte[] ActiveNodes;// []Checksum256 `json:"_active_nodes"`
    public ulong NodeCount;//   uint64        `json:"_node_count"`
}