using DeepReader.EosTypes;

namespace DeepReader.Types;

public class PendingSchedule
{
    public uint ScheduleLIBNum = 0;//uint32
    public Checksum256 ScheduleHash = string.Empty;//Checksum256
    public ProducerAuthoritySchedule Schedule = new ProducerAuthoritySchedule();//*ProducerScheduleOrAuthoritySchedule
}