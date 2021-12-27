namespace DeepReader.Types;

public class ProducerScheduleOrAuthoritySchedule
{
    // EOSIO 1.x
    public ProducerSchedule V1 = new ProducerSchedule();

    // EOSIO 2.x
    public ProducerAuthoritySchedule V2 = new ProducerAuthoritySchedule();
}