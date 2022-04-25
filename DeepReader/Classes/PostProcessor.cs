using DeepReader.Types;
using DeepReader.Types.FlattenedTypes;
using DeepReader.Types.Helpers;

namespace DeepReader.Classes
{
    internal class PostProcessor
    {
        static (FlattenedBlock, IEnumerable<FlattenedTransactionTrace>) Flatten(Block block)
        {
            var flattenedBlock = new FlattenedBlock()
            {
                Number = block.Number,
                TransactionIds = block.UnfilteredTransactionTraces.Select(ut => ut.Id).ToArray(),
                Id = block.Id,
                Producer = block.Header.Producer,
                ProducerSignature = block.ProducerSignature
            };

            return (flattenedBlock, block.UnfilteredTransactionTraces.Select(transactionTrace =>
                new FlattenedTransactionTrace
                {
                    ActionTraces = transactionTrace.ActionTraces.Select((actionTrace, actionIndex) => 
                        new FlattenedActionTrace()
                        {
                            AccountRamDeltas = actionTrace.AccountRamDeltas,
                            Act = actionTrace.Act,
                            Console = actionTrace.Console,
                            ContextFree = actionTrace.ContextFree,
                            DbOps = transactionTrace.DbOps.Where(dbOp => dbOp.ActionIndex == actionIndex).Select(dbOp =>
                                new FlattenedDbOp()
                                {
                                    Code = dbOp.Code,
                                    NewData = dbOp.NewData,
                                    NewPayer = dbOp.NewPayer,
                                    OldData = dbOp.OldData,
                                    OldPayer = dbOp.OldPayer,
                                    Operation = dbOp.Operation,
                                    PrimaryKey = SerializationHelper.PrimaryKeyToBytes(dbOp.PrimaryKey),
                                    Scope = dbOp.Scope,
                                    TableName = dbOp.TableName
                                }).ToArray(),
                            ElapsedUs = actionTrace.ElapsedUs,
                            RamOps = transactionTrace.RamOps.Where(ramOp => ramOp.ActionIndex == actionIndex).Select(ramOp =>
                                new FlattenedRamOp()
                                {
                                    Action = ramOp.Action,
                                    Delta = ramOp.Delta,
                                    Namespace = ramOp.Namespace,
                                    Operation = ramOp.Operation,
                                    Payer = ramOp.Payer,
                                    Usage = ramOp.Usage
                                }).ToArray(),
                            Receiver = actionTrace.Receiver,
                            ReturnValue = actionTrace.ReturnValue,
                            TableOps = transactionTrace.TableOps.Where(tableOp => tableOp.ActionIndex == actionIndex).Select(
                                tableOp => new FlattenedTableOp()
                                {
                                    Code = tableOp.Code,
                                    Operation = tableOp.Operation,
                                    Payer = tableOp.Payer,
                                    Scope = tableOp.Scope,
                                    TableName = tableOp.TableName,
                                }).ToArray(),
                        }
                    ).ToArray()
                }));
        }
    }
}
