using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.FlattenedTypes;

public struct FlattenedTransactionTrace
{
    // SHA-256 (FIPS 180-4) of the FCBUFFER-encoded packed transaction
    public TransactionId Id = Array.Empty<byte>();

    public uint BlockNum = 0;

    public long Elapsed = 0;

    public ulong NetUsage = 0;

    public FlattenedActionTrace[] ActionTraces = Array.Empty<FlattenedActionTrace>();

    public DbOp[] DbOps = Array.Empty<DbOp>();

    public TableOp[] TableOps = Array.Empty<TableOp>();

    public FlattenedTransactionTrace()
    {

    }

    public static int recovered = 0;

    public static FlattenedTransactionTrace ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new FlattenedTransactionTrace()
        {
            Id = reader.ReadBytes(32),
            BlockNum = reader.ReadUInt32(),
            Elapsed = reader.ReadInt64(),
            NetUsage = reader.ReadUInt64(),
        };

        Interlocked.Increment(ref recovered);

        var test = reader.ReadInt32();
        if (test > 400)
        {
            string a = "" + recovered;
        }
        obj.ActionTraces = new FlattenedActionTrace[test];
        for (int i = 0; i < obj.ActionTraces.Length; i++)
        {
            obj.ActionTraces[i] = FlattenedActionTrace.ReadFromBinaryReader(reader);
        }
        
        obj.DbOps = new DbOp[reader.ReadInt32()];
        for (int i = 0; i < obj.DbOps.Length; i++)
        {
            obj.DbOps[i] = DbOp.ReadFromBinaryReader(reader);
        }

        obj.TableOps = new TableOp[reader.ReadInt32()];
        for (int i = 0; i < obj.TableOps.Length; i++)
        {
            obj.TableOps[i] = TableOp.ReadFromBinaryReader(reader);
        }

        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.WriteTransactionId(Id);
        
        writer.Write(BlockNum); // TODO VARINT
        writer.Write(Elapsed); // TODO VARINT
        writer.Write(NetUsage); // TODO VARINT

        if (ActionTraces.Length > 10)
        {
            string test = "";
        }
        writer.Write(ActionTraces.Length);
        foreach (var actionTrace in ActionTraces)
        {
            actionTrace.WriteToBinaryWriter(writer);
        }

        writer.Write(DbOps.Length);
        foreach (var dbOp in DbOps)
        {
            dbOp.WriteToBinaryWriter(writer);
        }

        writer.Write(TableOps.Length);
        foreach (var tableOp in TableOps)
        {
            tableOp.WriteToBinaryWriter(writer);
        }
    }
}