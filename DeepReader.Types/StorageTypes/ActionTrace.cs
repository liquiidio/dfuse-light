using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Other;
using Action = DeepReader.Types.Eosio.Chain.Action;

namespace DeepReader.Types.StorageTypes
{
    public sealed class ActionTrace : PooledObject<ActionTrace>, IParentPooledObject<TransactionTrace>
    {
        public ulong GlobalSequence 
        {
            get
            {
                return Receipt.GlobalSequence;
            }
            set
            {
                Receipt.GlobalSequence = value;
            }
        }

        // executionindex

        public ActionReceipt Receipt { get; set; }

        public Name Receiver { get; set; } = Name.TypeEmpty;

        public Action Act { get; set; } = new();

        public List<RamOp> RamOps { get; set; } = new List<RamOp>();

        // dtrxOps

        public List<TableOp> TableOps { get; set; } = new List<TableOp>();
        public List<DbOp> DbOps { get; set; } = new List<DbOp>();

        public string Console { get; set; } = string.Empty;

        public bool ContextFree { get; set; } = false;

        public long ElapsedUs { get; set; } = 0;

//        public AccountDelta[] AccountRamDeltas { get; set; } = Array.Empty<AccountDelta>();

        public char[] ReturnValue { get; set; } = Array.Empty<char>(); // TODO, string?

        public bool IsNotify { get; set; } = false;

        public ulong[] CreatedActionIds { get; set; }

        public List<ActionTrace> CreatedActions { get; set; } = new List<ActionTrace>();

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

        public static ActionTrace ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
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

            var ramOpsCount = reader.ReadInt32();
            obj.RamOps = new List<RamOp>(ramOpsCount);
            for (int i = 0; i < ramOpsCount; i++)
            {
                obj.RamOps.Add(RamOp.ReadFromBinaryReader(reader));
            }

            var tableOpsCount = reader.ReadInt32();
            obj.TableOps = new List<TableOp>(tableOpsCount);
            for (int i = 0; i < tableOpsCount; i++)
            {
                obj.TableOps.Add(TableOp.ReadFromBinaryReader(reader));
            }

            var dbOpsCount = reader.ReadInt32();
            obj.DbOps = new List<DbOp>(dbOpsCount);
            for (int i = 0; i < dbOpsCount; i++)
            {
                obj.DbOps.Add(DbOp.ReadFromBinaryReader(reader));
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

        public void WriteToBinaryWriter(BinaryWriter writer)
        {
            Receiver.WriteToBinaryWriter(writer);

            Receipt.WriteToBinaryWriter(writer);

            Act.WriteToBinaryWriter(writer);
            
            writer.Write(ContextFree);
            writer.Write(ElapsedUs);
            writer.Write(Console);

            writer.Write(IsNotify);

            writer.Write(RamOps.Count);
            foreach (var ramOp in RamOps)
            {
                ramOp.WriteToBinaryWriter(writer);
            }

            writer.Write(TableOps.Count);
            foreach (var tableOp in TableOps)
            {
                tableOp.WriteToBinaryWriter(writer);
            }

            writer.Write(DbOps.Count);
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

            // as we return this Object to the pool we need to reset Lists and nullables;
            CreatorActionId = null;
            CreatorAction = null;
        }

        public void ReturnToPoolRecursive()
        {
            if(Receipt != null)
                ActionReceipt.ReturnToPool(Receipt);
            //            Action Act
            RamOps = new List<RamOp>();
            TableOps = new List<TableOp>();
            DbOps = new List<DbOp>();
            CreatedActions = new List<ActionTrace>();
            if (CreatorAction != null)
            {
                ActionTrace.ReturnToPool(CreatorAction);
                CreatorAction = null;
            }

            TypeObjectPool.Return(this);
        }
    }
}
