namespace DeepReader.Types;

public class PermissionLevel
{
    public string Actor;//                string   `protobuf:"bytes,1,opt,name=actor,proto3" json:"actor,omitempty"`
    public string Permission;//           string   `protobuf:"bytes,2,opt,name=permission,proto3" json:"permission,omitempty"`
    //XXX_NoUnkeyedLiteral struct{} `json:"-"`
    //XXX_unrecognized     []byte   `json:"-"`
    //XXX_sizecache        int32    `json:"-"`
}