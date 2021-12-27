using DeepReader.EosTypes;

namespace DeepReader.Types;

public class SignedTransaction : Transaction
{
    //    public Transaction Transaction = new Transaction();//*Transaction
    [SortOrder(10)]
    public IList<Signature> Signatures;

    [SortOrder(11)]
    public IList<Bytes> ContextFreeData; ///< for each context-free action, there is an entry here

    //public string[] Signatures = Array.Empty<string>();//[]string
    //public byte[][] ContextFreeData = Array.Empty<byte[]>();//[][]byte
}