namespace DeepReader.Types;

public class Action
{
    public string Account;//              string             `protobuf:"bytes,1,opt,name=account,proto3" json:"account,omitempty"`
    public string Name;//                 string             `protobuf:"bytes,2,opt,name=name,proto3" json:"name,omitempty"`
    public PermissionLevel[] Authorization;//        []*PermissionLevel `protobuf:"bytes,3,rep,name=authorization,proto3" json:"authorization,omitempty"`
    public string JsonData;//             string             `protobuf:"bytes,4,opt,name=json_data,json=jsonData,proto3" json:"json_data,omitempty"`
    public byte[] RawData;//              []byte             `protobuf:"bytes,5,opt,name=raw_data,json=rawData,proto3" json:"raw_data,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}           `json:"-"`
    //XXX_unrecognized     []byte             `json:"-"`
    //XXX_sizecache        int32              `json:"-"`
}