using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using Action = DeepReader.Types.Eosio.Chain.Action;

namespace DeepReader.Types.FlattenedTypes
{
    public class FlattenedActionTrace
    {
        public Name Receiver { get; set; } = Name.TypeEmpty;

        public Action Act { get; set; } = new();

        public bool ContextFree { get; set; } = false;

        public long ElapsedUs { get; set; } = 0;

        public string Console { get; set; } = string.Empty;

        public ulong GlobalSequence { get; set; } = 0;

        public AccountDelta[] AccountRamDeltas { get; set; } = Array.Empty<AccountDelta>();

        public FlattenedRamOp[] RamOps { get; set; } = Array.Empty<FlattenedRamOp>();

        public FlattenedDbOp[] DbOps { get; set; } = Array.Empty<FlattenedDbOp>();

        public FlattenedTableOp[] TableOps { get; set; } = Array.Empty<FlattenedTableOp>();

        public char[] ReturnValue { get; set; } = Array.Empty<char>(); // TODO, string?


        public FlattenedActionTrace()
        {

        }

        public static FlattenedActionTrace ReadFromBinaryReader(BinaryReader reader)
        {
            var obj = new FlattenedActionTrace()
            {
                Receiver = reader.ReadName(),
                Act = Action.ReadFromBinaryReader(reader),
                ContextFree = reader.ReadBoolean(),
                ElapsedUs = reader.ReadInt64(),
                Console = reader.ReadString(),
                GlobalSequence = (ulong)reader.Read7BitEncodedInt64()
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
            writer.Write7BitEncodedInt64((long)GlobalSequence);

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
