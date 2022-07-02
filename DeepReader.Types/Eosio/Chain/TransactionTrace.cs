using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Other;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public sealed class TransactionTrace : IEosioSerializable<TransactionTrace>, IFasterSerializable<TransactionTrace>
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
    public IList<ExtendedDbOp> DbOps { get; set; } = new List<ExtendedDbOp>();//[]*DBOp
                                                                              // List of deferred transactions operations this transaction entailed
    public IList<DTrxOp> DtrxOps { get; set; } = new List<DTrxOp>();//[]*DTrxOp
                                                                    // List of feature switching operations (changes to feature switches in
                                                                    // nodeos) this transaction entailed
    public IList<FeatureOp> FeatureOps { get; set; } = new List<FeatureOp>();//[]*FeatureOp
                                                                             // List of permission changes operations
    public IList<PermOp> PermOps { get; set; } = new List<PermOp>();//[]*PermOp
                                                                    // List of RAM consumption/redemption
    public IList<ExtendedRamOp> RamOps { get; set; } = new List<ExtendedRamOp>();//[]*RAMOp
                                                                                 // List of RAM correction operations (happens only once upon feature
                                                                                 // activation)
    public IList<RamCorrectionOp> RamCorrectionOps { get; set; } = new List<RamCorrectionOp>();//[]*RAMCorrectionOp
                                                                                               // List of changes to rate limiting values
    public IList<RlimitOp> RlimitOps { get; set; } = new List<RlimitOp>();//[]*RlimitOp
                                                                          // List of table creations/deletions
    public IList<ExtendedTableOp> TableOps { get; set; } = new List<ExtendedTableOp>();//[]*TableOp
                                                                                       // Tree of creation, rather than execution
    public IList<CreationTreeNode> CreationTreeRoots { get; set; } = new List<CreationTreeNode>();//[]*CreationFlatNode

    //public IList<CreationFlatNode> FlatCreationTree { get; set; } = new List<CreationFlatNode>();//[]*CreationFlatNode

    // Index within block's unfiltered execution traces
    public ulong Index { get; set; } = 0;

    public TransactionTrace()
    {
        Id = TransactionId.TypeEmpty;
        BlockTime = Timestamp.Zero;
        ActionTraces = Array.Empty<ActionTrace>();
    }

    public TransactionTrace(IBufferReader reader)
    {
        Id = TransactionId.DeserializeKey(reader);
        BlockNum = reader.ReadUInt32();
        BlockTime = Timestamp.ReadFromBinaryReader(reader);

        var readProducerBlockId = reader.ReadBoolean();
        if (readProducerBlockId)
            ProducerBlockId = Checksum256.ReadFromBinaryReader(reader);

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
        //        if (readFailedDtrxTrace) // TODO
        //            return;
        //            FailedDtrxTrace = ReadFromBinaryReader(reader);

        var readException = reader.ReadBoolean();
        if (readException)
            Exception = Except.ReadFromBinaryReader(reader);

        var readErrorCode = reader.ReadBoolean();
        if (readErrorCode)
            ErrorCode = reader.ReadUInt64();

    }

    public static TransactionTrace ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new TransactionTrace(reader);
    }

    public static TransactionTrace ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        var obj = new TransactionTrace()
        {
            Id = TransactionId.ReadFromFaster(reader),
            BlockNum = reader.ReadUInt32(),
            BlockTime = Timestamp.ReadFromFaster(reader)
        };

        var readProducerBlockId = reader.ReadBoolean();
        if (readProducerBlockId)
            obj.ProducerBlockId = Checksum256.ReadFromFaster(reader);

        var readReceipt = reader.ReadBoolean();
        if (readReceipt)
            obj.Receipt = TransactionReceiptHeader.ReadFromFaster(reader);

        obj.Elapsed = reader.ReadInt64();
        obj.NetUsage = reader.ReadUInt64();
        obj.Scheduled = reader.ReadBoolean();

        obj.ActionTraces = new ActionTrace[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.ActionTraces.Length; i++)
        {
            obj.ActionTraces[i] = ActionTrace.ReadFromFaster(reader);
        }

        var readAccountRamDelta = reader.ReadBoolean();
        if (readAccountRamDelta)
            obj.AccountRamDelta = AccountRamDelta.ReadFromFaster(reader);

        var readFailedDtrxTrace = reader.ReadBoolean();
        //        if (readFailedDtrxTrace) // TODO
        //            return;
        //            FailedDtrxTrace = ReadFromBinaryReader(reader);

        var readException = reader.ReadBoolean();
        if (readException)
            obj.Exception = Except.ReadFromFaster(reader);

        var readErrorCode = reader.ReadBoolean();
        if (readErrorCode)
            obj.ErrorCode = reader.ReadUInt64();

        return obj;
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

public sealed class Except : IEosioSerializable<Except>, IFasterSerializable<Except>
{
    public long Code;
    public string Name = string.Empty;
    public string Message = string.Empty;
    public ExceptLogMessage[] Stack = Array.Empty<ExceptLogMessage>();

    public static Except? ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        // TODO: (Corvin) 
        // Corvin: "This is something that was missing my version as well, need to do some research to understand how it's serialized"
        // Exceptions (Except) are only written to the dlog when we are in synch and receiving the Head-Block
        // - as long as we "replay" very old blocks Except? should always just be Null so returning Null should work here for now
        return null;
    }

    public static Except ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        throw new NotImplementedException();
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

public sealed class ExceptLogMessage
{
    public ExceptLogContext Context = new();
    public string Format = string.Empty;
    public string Data = string.Empty;// json.RawMessage
}

public sealed class ExceptLogContext
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

public sealed class CreationFlatNode
{
    public int WalkIndex = 0;
    public int CreatorActionIndex = 0;//int32
    public int ExecutionActionIndex = 0;//uint32
}