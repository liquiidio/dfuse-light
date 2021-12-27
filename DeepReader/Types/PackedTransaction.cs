namespace DeepReader.Types;

public class PackedTransaction
{
    public string[] Signatures = Array.Empty<string>();//[]string
    public uint Compression = 0;//uint32
    public byte[] PackedContextFreeData = Array.Empty<byte>();//[]byte
    public byte[] _PackedTransaction = Array.Empty<byte>();//[]byte
}