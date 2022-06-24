using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Other;
using Action = DeepReader.Types.Eosio.Chain.Action;

namespace DeepReader.Types.StorageTypes
{
    public sealed class ActionTrace : PooledObject<ActionTrace>, IParentPooledObject<TransactionTrace>, IEosioSerializable<ActionTrace>, IFasterSerializable<ActionTrace>
    {
        public ulong GlobalSequence => Receipt.GlobalSequence;

        // executionindex

        public ActionReceipt Receipt { get; set; }

        public Name Receiver { get; set; } = Name.TypeEmpty;

        public Action Act { get; set; } = new();

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

        public void CopyFrom(DeepReader.Types.Eosio.Chain.ActionTrace actionTrace)
        {
            Receipt = actionTrace.Receipt!;
            Receiver = actionTrace.Receiver;
            Act = actionTrace.Act;
            Console = actionTrace.Console;
            ContextFree = actionTrace.ContextFree;
            ElapsedUs = actionTrace.ElapsedUs;
            ReturnValue = actionTrace.ReturnValue;
        }

        public static ActionTrace ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
        {
            // when Faster wants to deserialize and Object, we take an Object from the Pool
            // when Faster evicts the Object we return it to the Pool
            var obj = TypeObjectPool.Get();

            obj.Receiver = Name.ReadFromBinaryReader(reader);
            obj.Receipt = ActionReceipt.ReadFromBinaryReader(reader);
            obj.Act = Action.ReadFromBinaryReader(reader);
            obj.ContextFree = reader.ReadBoolean();
            obj.ElapsedUs = reader.ReadInt64();
            obj.Console = reader.ReadString();
            obj.IsNotify = reader.ReadBoolean();

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
            else
                obj.CreatorActionId = null;

            return obj;
        }

        public static ActionTrace ReadFromFaster(BinaryReader reader, bool fromPool = true)
        {
            // when Faster wants to deserialize and Object, we take an Object from the Pool
            // when Faster evicts the Object we return it to the Pool
            var obj = TypeObjectPool.Get();

            obj.Receiver = Name.ReadFromFaster(reader);
            obj.Receipt = ActionReceipt.ReadFromFaster(reader);
            obj.Act = Action.ReadFromFaster(reader);
            obj.ContextFree = reader.ReadBoolean();
            obj.ElapsedUs = reader.ReadInt64();
            obj.Console = reader.ReadString();
            obj.IsNotify = reader.ReadBoolean();

            obj.RamOps = new RamOp[reader.ReadInt32()];
            for (int i = 0; i < obj.RamOps.Length; i++)
            {
                obj.RamOps[i] = RamOp.ReadFromFaster(reader);
            }

            obj.TableOps = new TableOp[reader.ReadInt32()];
            for (int i = 0; i < obj.TableOps.Length; i++)
            {
                obj.TableOps[i] = TableOp.ReadFromFaster(reader);
            }

            obj.DbOps = new DbOp[reader.ReadInt32()];
            for (int i = 0; i < obj.DbOps.Length; i++)
            {
                obj.DbOps[i] = DbOp.ReadFromFaster(reader);
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
            else
                obj.CreatorActionId = null;

            return obj;
        }

        public void WriteToFaster(BinaryWriter writer)
        {
            Receiver.WriteToFaster(writer);

            Receipt.WriteToFaster(writer);

            Act.WriteToFaster(writer);

            writer.Write(ContextFree);
            writer.Write(ElapsedUs);
            writer.Write(Console);

            writer.Write(IsNotify);

            writer.Write(RamOps.Length);
            foreach (var ramOp in RamOps)
            {
                ramOp.WriteToFaster(writer);
            }

            writer.Write(TableOps.Length);
            foreach (var tableOp in TableOps)
            {
                tableOp.WriteToFaster(writer);
            }

            writer.Write(DbOps.Length);
            foreach (var dbOp in DbOps)
            {
                dbOp.WriteToFaster(writer);
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

            // as we return this Object to the pool we need to reset Lists and nullables;
            CreatorActionId = null;
            CreatorAction = null;
        }

        public void ReturnToPoolRecursive()
        {
            if(Receipt != null)
                ActionReceipt.ReturnToPool(Receipt);
//            Action Act
            RamOps = Array.Empty<RamOp>();
            TableOps = Array.Empty<TableOp>();
            DbOps = Array.Empty<DbOp>();
            CreatedActions = Array.Empty<ActionTrace>();
            if (CreatorAction != null)
            {
                ActionTrace.ReturnToPool(CreatorAction);
                CreatorAction = null;
            }

            TypeObjectPool.Return(this);
        }
    }
}
