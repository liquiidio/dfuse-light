using System.Text.Json.Serialization;

namespace DeepReader.Types;

public class CreationOp
{
    public CreationOpKind Kind = CreationOpKind.UNKNOWN; // ROOT, NOTIFY, CFA_INLINE, INLINE
    public int ActionIndex = 0;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CreationOpKind : byte
{
    UNKNOWN,
    ROOT, 
    NOTIFY, 
    CFA_INLINE, // ContextFreeAction_Inline
    INLINE
}