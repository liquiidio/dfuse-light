namespace DeepReader.Types;

public class PendingSchedule
{
    public uint ScheduleLIBNum;// uint32                               `json:"schedule_lib_num"`
    public byte[] ScheduleHash;//   Checksum256                          `json:"schedule_hash"`
    public ProducerScheduleOrAuthoritySchedule Schedule;//       *ProducerScheduleOrAuthoritySchedule `json:"schedule"`
}