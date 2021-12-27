namespace DeepReader.Types;

public class BlockHeader
{
    public Timestamp Timestamp = new Timestamp();//*timestamp.Timestamp
    public string Producer = string.Empty;//string
    public uint Confirmed = 0;//uint32
    public string Previous = string.Empty;//string
    public byte[] TransactionMroot = Array.Empty<byte>();//[]byte
    public byte[] ActionMroot = Array.Empty<byte>();//[]byte
    public uint ScheduleVersion = 0;//uint32
    public Extension[] HeaderExtensions = Array.Empty<Extension>();//[]*Extension
    // EOSIO 1.x only
    //
    // A change to producer schedule was reported as a `NewProducers` field on the
    // `BlockHeader` in EOSIO 1.x. In EOSIO 2.x, when feature `WTMSIG_BLOCK_SIGNATURES`
    // is activated, the `NewProducers` field is not present anymore and the schedule change
    // is reported through a `BlockHeaderExtension` on the the `BlockHeader` struct.
    //
    // If you need to access the old value, you can
    public ProducerSchedule NewProducersV1 = new ProducerSchedule();//*ProducerSchedule
}