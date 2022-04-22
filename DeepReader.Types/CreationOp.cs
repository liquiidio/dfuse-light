namespace DeepReader.Types;

public class CreationOp
{
    public CreationOpKind Kind = CreationOpKind.UNKNOWN; // ROOT, NOTIFY, CFA_INLINE, INLINE
    public int ActionIndex = 0;
}

public enum CreationOpKind : byte
{
    UNKNOWN,
    ROOT, 
    NOTIFY, 
    CFA_INLINE, 
    INLINE
}