namespace DeepReader.Types;

public class BlockRootMerkle
{
    public uint NodeCount = 0;//uint32
    public byte[][] ActiveNodes = Array.Empty<byte[]>();//[][]byte
}