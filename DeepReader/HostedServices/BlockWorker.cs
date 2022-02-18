using System.Text.Json;
using System.Threading.Channels;
using DeepReader.Apis.GraphQl.Queries;
using DeepReader.Storage;
using DeepReader.Types;
using DeepReader.Types.FlattenedTypes;
using DeepReader.Types.Helpers;
using Serilog;

namespace DeepReader.HostedServices;

public class BlockWorker : BackgroundService
{
    private readonly ChannelReader<Block> _blocksChannel;

    private readonly IStorageAdapter _storageAdapter;

    public BlockWorker(ChannelReader<Block> blocksChannel, IStorageAdapter storageAdapter)
    {
        _blocksChannel = blocksChannel;
        _storageAdapter = storageAdapter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ProcessBlocks(stoppingToken);
    }

    private async Task ProcessBlocks(CancellationToken cancellationToken)
    {
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            IncludeFields = true,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            MaxDepth = 0
        };

        bool test = true;
        await foreach (var block in _blocksChannel.ReadAllAsync(cancellationToken))
        {
            try
            {
                if(block.Number % 1000 == 0)
                    Log.Information($"got block {block.Number}");

                foreach (var setAbiAction in block.UnfilteredTransactionTraces.SelectMany(utt => utt.ActionTraces.Where(at => at.Act.Account == "eosio" && at.Act.Name == "setabi")))
                {
                    Log.Information($"got abi for {setAbiAction.Act.Account} at {block.Number}");

                    var abi = DeepMindDeserializer.DeepMindDeserializer.Deserialize<Abi>(setAbiAction.Act.Data);
                    if (abi != null && abi.AbiActions.Length > 0 || abi.AbiStructs.Length > 0)
                    {
                        Log.Information(JsonSerializer.Serialize(abi, jsonSerializerOptions));
                    }
                }

                var (flattenedBlock, flattenedTransactionTraces) = Flatten(block);

                await _storageAdapter.StoreBlockAsync(flattenedBlock);

                foreach (var flattenedTransactionTrace in flattenedTransactionTraces)
                {
                    await _storageAdapter.StoreTransactionAsync(flattenedTransactionTrace);
                }
            }
            catch (Exception e)
            {
                Log.Error(e,"");
            }
        }
    }

    private (FlattenedBlock, IEnumerable<FlattenedTransactionTrace>) Flatten(Block block)
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
                BlockNum = block.Number,
                DbOps = transactionTrace.DbOps.ToArray(),
                Elapsed = transactionTrace.Elapsed,
                Id = transactionTrace.Id,
                NetUsage = transactionTrace.NetUsage,
                TableOps = transactionTrace.TableOps.ToArray(),
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