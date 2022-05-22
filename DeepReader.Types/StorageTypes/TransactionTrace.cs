using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.StorageTypes;

public class TransactionTrace
{
    // SHA-256 (FIPS 180-4) of the FCBUFFER-encoded packed transaction
    public TransactionId Id { get; set; } = Array.Empty<byte>();

    public uint BlockNum { get; set; } = 0;

    public Block Block { get; set; }

    // Status

    public TransactionReceiptHeader Receipt { get; set; }

    public long Elapsed { get; set; } = 0;

    public ulong NetUsage { get; set; } = 0;

    public bool Scheduled { get; set; } = false;

    public ActionTrace[] ActionTraces { get; set; } = Array.Empty<ActionTrace>();

    public ulong[] ActionTraceIds { get; set; } = Array.Empty<ulong>();

    public TransactionTrace()
    {

    }

    public TransactionTrace(DeepReader.Types.Eosio.Chain.TransactionTrace transactionTrace)
    {
        Id = transactionTrace.Id;
        BlockNum = transactionTrace.BlockNum;
        Receipt = transactionTrace.Receipt!;
        Elapsed = transactionTrace.Elapsed;
        NetUsage = transactionTrace.NetUsage;
        Scheduled = transactionTrace.Scheduled;
    }

    public static TransactionTrace ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new TransactionTrace()
        {
            Id = reader.ReadBytes(32),
            BlockNum = reader.ReadUInt32(),
            Elapsed = reader.ReadInt64(),
            NetUsage = reader.ReadUInt64(),
        };

        obj.ActionTraces = new ActionTrace[reader.ReadInt32()];
        for (int i = 0; i < obj.ActionTraces.Length; i++)
        {
            obj.ActionTraces[i] = ActionTrace.ReadFromBinaryReader(reader);
        }

        //obj.DbOps = new DbOp[reader.ReadInt32()];
        //for (int i = 0; i < obj.DbOps.Length; i++)
        //{
        //    obj.DbOps[i] = DbOp.ReadFromBinaryReader(reader);
        //}

        //obj.TableOps = new TableOp[reader.ReadInt32()];
        //for (int i = 0; i < obj.TableOps.Length; i++)
        //{
        //    obj.TableOps[i] = TableOp.ReadFromBinaryReader(reader);
        //}

        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.WriteTransactionId(Id);

        writer.Write(BlockNum); // TODO VARINT
        writer.Write(Elapsed); // TODO VARINT
        writer.Write(NetUsage); // TODO VARINT

        writer.Write(ActionTraces.Length);
        foreach (var actionTrace in ActionTraces)
        {
            actionTrace.WriteToBinaryWriter(writer);
        }

        //writer.Write(DbOps.Length);
        //foreach (var dbOp in DbOps)
        //{
        //    dbOp.WriteToBinaryWriter(writer);
        //}

        //writer.Write(TableOps.Length);
        //foreach (var tableOp in TableOps)
        //{
        //    tableOp.WriteToBinaryWriter(writer);
        //}
    }
}