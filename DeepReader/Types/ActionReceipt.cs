namespace DeepReader.Types;

public class ActionReceipt
{
    public string Receiver = string.Empty;//string
    public string Digest = string.Empty;//string
    public ulong GlobalSequence = 0;//uint64
    public AuthSequence[] AuthSequence = Array.Empty<AuthSequence>();//[]*AuthSequence
    public ulong RecvSequence = 0;//uint64
    public ulong CodeSequence = 0;//uint64
    public ulong AbiSequence = 0;//uint64
}