namespace DeepReader.Types;

public class ProducerSchedule
{
    public uint Version;//              uint32         `protobuf:"varint,1,opt,name=version,proto3" json:"version,omitempty"`
    public ProducerKey[] Producers;//            []*ProducerKey `protobuf:"bytes,2,rep,name=producers,proto3" json:"producers,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}       `json:"-"`
    //XXX_unrecognized     []byte         `json:"-"`
    //XXX_sizecache        int32          `json:"-"`
}