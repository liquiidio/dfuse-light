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

        public FlattenedRamOp[] RamOps;

        public FlattenedDbOp[] DbOps;

        public FlattenedTableOp[] TableOps;

        public char[] ReturnValue = Array.Empty<char>();    // TODO, string?

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

            return obj;
        }

        public void WriteToBinaryWriter(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
