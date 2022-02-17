using DeepReader.Types.Enums;

namespace DeepReader.Types;

public class PermOp
{
    public PermOpOperation Operation = PermOpOperation.UNKNOWN;//PermOp_Operation
    public uint ActionIndex = 0;//uint32
    public PermissionObject OldPerm = new();//*PermissionObject
    public PermissionObject NewPerm = new();//*PermissionObject
}