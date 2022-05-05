using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public class TransactionTrace : IEosioSerializable<TransactionTrace>
{
    // SHA-256 (FIPS 180-4) of the FCBUFFER-encoded packed transaction
    public TransactionId Id;
    // Reference to the block number in which this transaction was executed.
    public uint BlockNum;
    // Reference to the block time this transaction was executed in
    public Timestamp BlockTime;
    // Reference to the block ID this transaction was executed in
    public Checksum256? ProducerBlockId = Checksum256.TypeEmpty;
    // Receipt of execution of this transaction
    public TransactionReceiptHeader? Receipt;
    public long Elapsed;
    public ulong NetUsage;
    // Whether this transaction was taken from a scheduled transactions pool for
    // execution (delayed)
    public bool Scheduled;
    // Traces of each action within the transaction, including all notified and
    // nested actions.
    public ActionTrace[] ActionTraces;

    // Account RAM Delta - ignored in dfuse
    public AccountRamDelta? AccountRamDelta;

    // Trace of a failed deferred transaction, if any.
    public TransactionTrace? FailedDtrxTrace;

    // Exception leading to the failed dtrx trace.
    public Except? Exception;

    public ulong? ErrorCode;

    // List of database operations this transaction entailed
    public IList<DbOp> DbOps { get; set; } = new List<DbOp>();//[]*DBOp
                                                              // List of deferred transactions operations this transaction entailed
    public IList<DTrxOp> DtrxOps { get; set; } = new List<DTrxOp>();//[]*DTrxOp
                                                                    // List of feature switching operations (changes to feature switches in
                                                                    // nodeos) this transaction entailed
    public IList<FeatureOp> FeatureOps { get; set; } = new List<FeatureOp>();//[]*FeatureOp
                                                                             // List of permission changes operations
    public IList<PermOp> PermOps { get; set; } = new List<PermOp>();//[]*PermOp
                                                                    // List of RAM consumption/redemption
    public IList<RamOp> RamOps { get; set; } = new List<RamOp>();//[]*RAMOp
                                                                 // List of RAM correction operations (happens only once upon feature
                                                                 // activation)
    public IList<RamCorrectionOp> RamCorrectionOps { get; set; } = new List<RamCorrectionOp>();//[]*RAMCorrectionOp
                                                                                               // List of changes to rate limiting values
    public IList<RlimitOp> RlimitOps { get; set; } = new List<RlimitOp>();//[]*RlimitOp
                                                                          // List of table creations/deletions
    public IList<TableOp> TableOps { get; set; } = new List<TableOp>();//[]*TableOp
                                                                       // Tree of creation, rather than execution
    public IList<CreationFlatNode> CreationTree { get; set; } = new List<CreationFlatNode>();//[]*CreationFlatNode

    // Index within block's unfiltered execution traces
    public ulong Index { get; set; } = 0;

    public TransactionTrace()
    {
        Id = TransactionId.TypeEmpty;
        BlockTime = Timestamp.Zero;
        ActionTraces = Array.Empty<ActionTrace>();
    }

    public TransactionTrace(BinaryReader reader)
    {
        Id = reader.ReadTransactionId();
        BlockNum = reader.ReadUInt32();
        BlockTime = reader.ReadTimestamp();

        var readProducerBlockId = reader.ReadBoolean();

        if (readProducerBlockId)
            ProducerBlockId = reader.ReadChecksum256();

        var readReceipt = reader.ReadBoolean();

        if (readReceipt)
            Receipt = TransactionReceiptHeader.ReadFromBinaryReader(reader);

        Elapsed = reader.ReadInt64();
        NetUsage = reader.ReadUInt64();
        Scheduled = reader.ReadBoolean();

        ActionTraces = new ActionTrace[reader.Read7BitEncodedInt()];
        for (int i = 0; i < ActionTraces.Length; i++)
        {
            ActionTraces[i] = ActionTrace.ReadFromBinaryReader(reader);
        }

        var readAccountRamDelta = reader.ReadBoolean();

        if (readAccountRamDelta)
            AccountRamDelta = AccountRamDelta.ReadFromBinaryReader(reader);

        var readFailedDtrxTrace = reader.ReadBoolean();
        // Todo @corvin from haron
        // Haron: "This is a weird field. It is of the same type as this, how does it work?
        //if (readFailedDtrxTrace)
        //	FailedDtrxTrace = transactionTrace

        var readException = reader.ReadBoolean();
        if (readException)
            Exception = Except.ReadFromBinaryReader(reader);

        var readErrorCode = reader.ReadBoolean();
        if (readErrorCode)
            ErrorCode = reader.ReadUInt64();

    }

    public static TransactionTrace ReadFromBinaryReader(BinaryReader reader)
    {
        return new TransactionTrace(reader);
    }
}

public class Except : IEosioSerializable<Except>
{
    public long Code;
    public string Name = string.Empty;
    public string Message = string.Empty;
    public ExceptLogMessage[] Stack = Array.Empty<ExceptLogMessage>();

    public static Except? ReadFromBinaryReader(BinaryReader reader)
    {
        // TODO: (Corvin) 
        // Corvin: "This is something that was missing my version as well, need to do some research to understand how it's serialized"
        // Exceptions (Except) are only written to the dlog when we are in synch and receiving the Head-Block
        // - as long as we "replay" very old blocks Except? should always just be Null so returning Null should work here for now
        return null;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

public class ExceptLogMessage
{
    public ExceptLogContext Context = new();
    public string Format = string.Empty;
    public string Data = string.Empty;// json.RawMessage
}

public class ExceptLogContext
{
    public byte Level;//ExceptLogLevel
    public string File = string.Empty;
    public ulong Line;
    public string Method = string.Empty;
    public string Hostname = string.Empty;
    public string ThreadName = string.Empty;
    public Timestamp Timestamp = Timestamp.Zero;//JSONTime
    public ExceptLogContext? Context;
}

public class CreationFlatNode
{
    public int WalkIndex = 0;
    public int CreatorActionIndex = 0;//int32
    public int ExecutionActionIndex = 0;//uint32
}