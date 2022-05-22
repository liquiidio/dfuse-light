using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using Action = DeepReader.Types.Eosio.Chain.Action;

namespace DeepReader.Types.StorageTypes
{
    public class ActionTrace
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

        public ActionTrace[] CreatedActions { get; set; }

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
                Act = Action.ReadFromBinaryReader(reader),
                ContextFree = reader.ReadBoolean(),
                ElapsedUs = reader.ReadInt64(),
                Console = reader.ReadString()
            };

            //obj.AccountRamDeltas = new AccountDelta[reader.ReadInt32()];
            //for (int i = 0; i < obj.AccountRamDeltas.Length; i++)
            //{
            //    obj.AccountRamDeltas[i] = AccountDelta.ReadFromBinaryReader(reader);
            //}

            obj.RamOps = new RamOp[reader.ReadInt32()];
            for (int i = 0; i < obj.RamOps.Length; i++)
            {
                obj.RamOps[i] = RamOp.ReadFromBinaryReader(reader);
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

            obj.ReturnValue = new char[reader.ReadInt32()];
            for (int i = 0; i < obj.ReturnValue.Length; i++)
            {
                obj.ReturnValue[i] = reader.ReadChar();
            }

            return obj;
        }

        public void WriteToBinaryWriter(BinaryWriter writer)
        {
            writer.WriteName(Receiver);

            Act.WriteToBinaryWriter(writer);
            
            writer.Write(ContextFree);
            writer.Write(ElapsedUs);
            writer.Write(Console);

            //writer.Write(AccountRamDeltas.Length);
            //foreach (var accountRamDelta in AccountRamDeltas)
            //{
            //    accountRamDelta.WriteToBinaryWriter(writer);
            //}

            writer.Write(RamOps.Length);
            foreach (var ramOp in RamOps)
            {
                ramOp.WriteToBinaryWriter(writer);
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

            writer.Write(ReturnValue.Length);
            foreach (var returnVal in ReturnValue)
            {
                writer.Write(returnVal);
            }
        }
    }
}
