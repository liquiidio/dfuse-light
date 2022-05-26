using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using Action = DeepReader.Types.Eosio.Chain.Action;

namespace DeepReader.Types.StorageTypes
{
    public sealed class ActionTrace
    {
        public ulong GlobalSequence => Receipt.GlobalSequence;

        // executionindex

        public ActionReceipt Receipt { get; set; }

        public Name Receiver { get; set; } = Name.TypeEmpty;

        public Action Act { get; set; } = new();

        //public Name Account;

        //public Name Name;

        //public PermissionLevel[] Authorization;

        //public ActionDataBytes Data;

        // json

        // hexdata

        // end act

        public RamOp[] RamOps { get; set; } = Array.Empty<RamOp>();

        // dtrxOps

        public TableOp[] TableOps { get; set; } = Array.Empty<TableOp>();
        public DbOp[] DbOps { get; set; } = Array.Empty<DbOp>();

        public string Console { get; set; } = string.Empty;

        public bool ContextFree { get; set; } = false;

        public long ElapsedUs { get; set; } = 0;

//        public AccountDelta[] AccountRamDeltas { get; set; } = Array.Empty<AccountDelta>();

        public char[] ReturnValue { get; set; } = Array.Empty<char>(); // TODO, string?

        public bool IsNotify { get; set; } = false;

        public ulong[] CreatedActionIds { get; set; }

        public ActionTrace[] CreatedActions { get; set; } = Array.Empty<ActionTrace>();

        public ulong? CreatorActionId { get; set; }

        public ActionTrace? CreatorAction { get; set; }

        // closestUnnotifiedAncestorAction

        public ActionTrace()
        {

        }

        public ActionTrace(DeepReader.Types.Eosio.Chain.ActionTrace actionTrace)
        {
            Receipt = actionTrace.Receipt!;
            Receiver = actionTrace.Receiver;
            Act = actionTrace.Act;
            Console = actionTrace.Console;
            ContextFree = actionTrace.ContextFree;
            ElapsedUs = actionTrace.ElapsedUs;
            ReturnValue = actionTrace.ReturnValue;
        }

        public static ActionTrace ReadFromBinaryReader(BinaryReader reader)
        {
            var obj = new ActionTrace()
            {
                Receiver = reader.ReadName(),
                Receipt = ActionReceipt.ReadFromBinaryReader(reader),
                Act = Action.ReadFromBinaryReader(reader),
                ContextFree = reader.ReadBoolean(),
                ElapsedUs = reader.ReadInt64(),
                Console = reader.ReadString(),
                IsNotify = reader.ReadBoolean(),
            };

            obj.RamOps = new RamOp[reader.ReadInt32()];
            for (int i = 0; i < obj.RamOps.Length; i++)
            {
                obj.RamOps[i] = RamOp.ReadFromBinaryReader(reader);
            }

            obj.TableOps = new TableOp[reader.ReadInt32()];
            for (int i = 0; i < obj.TableOps.Length; i++)
            {
                obj.TableOps[i] = TableOp.ReadFromBinaryReader(reader);
            }

            obj.DbOps = new DbOp[reader.ReadInt32()];
            for (int i = 0; i < obj.DbOps.Length; i++)
            {
                obj.DbOps[i] = DbOp.ReadFromBinaryReader(reader);
            }

            obj.ReturnValue = new char[reader.ReadInt32()];
            for (int i = 0; i < obj.ReturnValue.Length; i++)
            {
                obj.ReturnValue[i] = reader.ReadChar();
            }

            obj.CreatedActionIds = new ulong[reader.ReadInt32()];
            for (int i = 0; i < obj.CreatedActionIds.Length; i++)
            {
                obj.CreatedActionIds[i] = reader.ReadUInt64();
            }

            var hasCreatorActionId = reader.ReadBoolean();
            if (hasCreatorActionId)
                obj.CreatorActionId = reader.ReadUInt64();

            return obj;
        }

        public void WriteToBinaryWriter(BinaryWriter writer)
        {
            writer.WriteName(Receiver);

            Receipt.WriteToBinaryWriter(writer);

            Act.WriteToBinaryWriter(writer);
            
            writer.Write(ContextFree);
            writer.Write(ElapsedUs);
            writer.Write(Console);

            writer.Write(IsNotify);

            writer.Write(RamOps.Length);
            foreach (var ramOp in RamOps)
            {
                ramOp.WriteToBinaryWriter(writer);
            }

            writer.Write(TableOps.Length);
            foreach (var tableOp in TableOps)
            {
                tableOp.WriteToBinaryWriter(writer);
            }

            writer.Write(DbOps.Length);
            foreach (var dbOp in DbOps)
            {
                dbOp.WriteToBinaryWriter(writer);
            }

            writer.Write(ReturnValue.Length);
            foreach (var returnVal in ReturnValue)
            {
                writer.Write(returnVal);
            }

            writer.Write(CreatedActionIds.Length);
            foreach (var createdActionId in CreatedActionIds)
            {
                writer.Write(createdActionId);
            }

            writer.Write(CreatorActionId.HasValue);
            if (CreatorActionId.HasValue)
                writer.Write(CreatorActionId.Value);
        }
    }
}
