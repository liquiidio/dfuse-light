namespace DeepReader;

public class PendingProducerSchedule
{
    public uint ScheduleLibNum;// uint32 `protobuf:"varint,1,opt,name=schedule_lib_num,json=scheduleLibNum,proto3" json:"schedule_lib_num,omitempty"`
    public byte[] ScheduleHash;//   []byte `protobuf:"bytes,2,opt,name=schedule_hash,json=scheduleHash,proto3" json:"schedule_hash,omitempty"`
    // See Block#active_schedule_v1 for further details, this is the same change
    // as the active schedule, but applied to the pending field.
    public ProducerSchedule ScheduleV1;// *ProducerSchedule `protobuf:"bytes,3,opt,name=schedule_v1,json=scheduleV1,proto3" json:"schedule_v1,omitempty"`
    // See Block#active_schedule_v2 for further details, this is the same change
    // as the active schedule, but applied to the pending field.
    public ProducerAuthoritySchedule ScheduleV2;//           *ProducerAuthoritySchedule `protobuf:"bytes,4,opt,name=schedule_v2,json=scheduleV2,proto3" json:"schedule_v2,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}                   `json:"-"`
    //XXX_unrecognized     []byte                     `json:"-"`
    //XXX_sizecache        int32                      `json:"-"`
}