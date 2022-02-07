using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Types;

public class PendingProducerSchedule
{
    public uint ScheduleLibNum = 0;//uint32
    public byte[] ScheduleHash = Array.Empty<byte>();//[]byte
    // See Block#active_schedule_v1 for further details, this is the same change
    // as the active schedule, but applied to the pending field.
    public ProducerSchedule ScheduleV1 = new ProducerSchedule();//*ProducerSchedule
    // See Block#active_schedule_v2 for further details, this is the same change
    // as the active schedule, but applied to the pending field.
    public ProducerAuthoritySchedule ScheduleV2 = new ProducerAuthoritySchedule();//*ProducerAuthoritySchedule
}