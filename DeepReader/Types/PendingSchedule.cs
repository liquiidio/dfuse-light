namespace DeepReader.Types;

public class PendingSchedule
{
    public uint ScheduleLIBNum = 0;//uint32
    public byte[] ScheduleHash = Array.Empty<byte>();//Checksum256
    public ProducerScheduleOrAuthoritySchedule Schedule = new ProducerScheduleOrAuthoritySchedule();//*ProducerScheduleOrAuthoritySchedule
}