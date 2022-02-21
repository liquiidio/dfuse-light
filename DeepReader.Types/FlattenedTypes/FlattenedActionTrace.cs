using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using Action = DeepReader.Types.Eosio.Chain.Action;

namespace DeepReader.Types.FlattenedTypes
{
    public struct FlattenedActionTrace
    {
        public Name Receiver = Name.Empty;

        public Action Act = new();

        public bool ContextFree = false;

        public long ElapsedUs = 0;

        public string Console = string.Empty;

        public AccountDelta[] AccountRamDeltas = Array.Empty<AccountDelta>();

        public FlattenedRamOp[] RamOps = Array.Empty<FlattenedRamOp>();

        public FlattenedDbOp[] DbOps = Array.Empty<FlattenedDbOp>();

        public FlattenedTableOp[] TableOps = Array.Empty<FlattenedTableOp>();

        public char[] ReturnValue = Array.Empty<char>(); // TODO, string?


        public FlattenedActionTrace()
        {

        }

        public static FlattenedActionTrace ReadFromBinaryReader(BinaryReader reader)
        {
            var obj = new FlattenedActionTrace()
            {
                Receiver = reader.ReadUInt64(),
                Act = Action.ReadFromBinaryReader(reader),
                ContextFree = reader.ReadBoolean(),
                ElapsedUs = reader.ReadInt64(),
                Console = reader.ReadString()
            };

            obj.AccountRamDeltas = new AccountDelta[reader.ReadInt32()];
            for (int i = 0; i < obj.AccountRamDeltas.Length; i++)
            {
                obj.AccountRamDeltas[i] = AccountDelta.ReadFromBinaryReader(reader);
            }

            obj.RamOps = new FlattenedRamOp[reader.ReadInt32()];
            for (int i = 0; i < obj.RamOps.Length; i++)
            {
                obj.RamOps[i] = FlattenedRamOp.ReadFromBinaryReader(reader);
            }

            obj.DbOps = new FlattenedDbOp[reader.ReadInt32()];
            for (int i = 0; i < obj.DbOps.Length; i++)
            {
                obj.DbOps[i] = FlattenedDbOp.ReadFromBinaryReader(reader);
            }

            obj.TableOps = new FlattenedTableOp[reader.ReadInt32()];
            for (int i = 0; i < obj.TableOps.Length; i++)
            {
                obj.TableOps[i] = FlattenedTableOp.ReadFromBinaryReader(reader);
            }

            obj.ReturnValue = new char[reader.ReadInt32()];
            for (int i = 0; i < obj.TableOps.Length; i++)
            {
                obj.ReturnValue[i] = reader.ReadChar();
            }

            return obj;
        }

        public void WriteToBinaryWriter(BinaryWriter writer)
        {
            writer.Write(Receiver.Binary);
            Act.WriteToBinaryWriter(writer);
            writer.Write(ContextFree);
            writer.Write(ElapsedUs);
            writer.Write(Console);

            writer.Write(AccountRamDeltas.Length);
            foreach (var accountRamDelta in AccountRamDeltas)
            {
                accountRamDelta.WriteToBinaryWriter(writer);
            }

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
