using System.Diagnostics;
using System.Threading.Channels;
using DeepReader.Options;
using DeepReader.Storage;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.FlattenedTypes;
using DeepReader.Types.Helpers;
using DeepReader.Types.Other;
using KGySoft.CoreLibraries;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Prometheus;
using Sentry;
using Serilog;

namespace DeepReader.HostedServices;

public class BlockWorker : BackgroundService
{
    private readonly ChannelReader<Block> _blocksChannel;
    private readonly ChannelReader<List<IList<StringSegment>>> _blockSegmentsListChannel;

    private readonly IStorageAdapter _storageAdapter;

    private MindReaderOptions _mindReaderOptions;
    private DeepReaderOptions _deepReaderOptions;

    private readonly ParallelOptions _blockFlatteningParallelOptions;

    private static readonly Histogram BlocksChannelSize = Metrics.CreateHistogram("deepreader_blockworker_block_channel_size", "The current size of the channel size in block worker");

    private readonly Func<FlattenedTransactionTrace, bool> _filterEmptyTransactionsFilter;

    private readonly Func<ActionTrace, bool> _actionFilter;
    private readonly Func<DbOp, bool> _deltaFilter;

    private readonly ObjectPool<Block> _blockPool;

    public BlockWorker(ChannelReader<Block> blocksChannel,
        ChannelReader<List<IList<StringSegment>>> blockSegmentsListChannel, 
        IStorageAdapter storageAdapter,
        IOptionsMonitor<MindReaderOptions> mindReaderOptionsMonitor,
        IOptionsMonitor<DeepReaderOptions> deepReaderOptionsMonitor,
        ObjectPool<Block> blockPool)
    {
        _mindReaderOptions = mindReaderOptionsMonitor.CurrentValue;
        mindReaderOptionsMonitor.OnChange(OnMindReaderOptionsChanged);

        _deepReaderOptions = deepReaderOptionsMonitor.CurrentValue;
        deepReaderOptionsMonitor.OnChange(OnDeepReaderOptionsChanged);

        _blockFlatteningParallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = _deepReaderOptions.FlatteningMaxDegreeOfParallelism
        };

        _blocksChannel = blocksChannel;
        _blockSegmentsListChannel = blockSegmentsListChannel;
        _storageAdapter = storageAdapter;

        _filterEmptyTransactionsFilter = _deepReaderOptions.FilterEmptyTransactions
            ? transactionTrace => transactionTrace.ActionTraces.Length > 0
            : _ => true;

        _actionFilter = _deepReaderOptions.Filter.BuildActionFilter();
        _deltaFilter = _deepReaderOptions.Filter.BuildDeltaFilter();

        _blockPool = blockPool;
    }

    private void OnMindReaderOptionsChanged(MindReaderOptions newOptions)
    {
        _mindReaderOptions = newOptions;
    }

    private void OnDeepReaderOptionsChanged(DeepReaderOptions newOptions)
    {
        _deepReaderOptions = newOptions;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Thread.CurrentThread.Name = $"BlockWorker";

        await ProcessBlocks(stoppingToken);
    }

    private async Task ProcessBlocks(CancellationToken cancellationToken)
    {
        //var jsonSerializerOptions = new JsonSerializerOptions()
        //{
        //    IncludeFields = true,
        //    IgnoreReadOnlyFields = false,
        //    IgnoreReadOnlyProperties = false,
        //    MaxDepth = Int32.MaxValue
        //};

        if (_blocksChannel.CanCount)
            BlocksChannelSize.Observe(_blocksChannel.Count);

        await foreach (var block in _blocksChannel.ReadAllAsync(cancellationToken))
        {
            try
            {
                if (block.Number % 1000 == 0)
                {
                    if (_blocksChannel.CanCount)
                        Log.Information($"blocks-channel-size: {_blocksChannel.Count}");

                    if (_blockSegmentsListChannel.CanCount)
                        Log.Information($"segment-channel-size: {_blockSegmentsListChannel.Count}");

                    Log.Information($"got block {block.Number}");
                    Log.Information($"Current Threads: {Process.GetCurrentProcess().Threads.Count}");
                }

                //foreach (var setAbiAction in block.UnfilteredTransactionTraces.SelectMany(utt => utt.ActionTraces.Where(at => at.Act.Account == "eosio" && at.Act.Name == "setabi")))
                //{
                //    Log.Information($"got abi for {setAbiAction.Act.Account} at {block.Number}");

                //    var abi = DeepMindDeserializer.DeepMindDeserializer.Deserialize<Abi>(setAbiAction.Act.Data);
                //    if (abi != null && abi.AbiActions.Length > 0 || abi.AbiStructs.Length > 0)
                //    {
                //        Log.Information(JsonSerializer.Serialize(abi, jsonSerializerOptions));
                //    }
                //}

                var (flattenedBlock, flattenedTransactionTraces) = await FlattenAsync(block);

                await _storageAdapter.StoreBlockAsync(flattenedBlock);

                await Parallel.ForEachAsync(flattenedTransactionTraces, _blockFlatteningParallelOptions,
                    async (flattenedTransactionTrace, _) =>
                    {
                        await _storageAdapter.StoreTransactionAsync(
                            flattenedTransactionTrace); // TODO cancellationToken
                    });

                await TestAbiDeserializer(flattenedTransactionTraces);
            }
            catch (Exception e)
            {
                Log.Error(e, "");
            }
            finally
            {
                _blockPool.Return(block);
            }
        }
    }

    private Task<(FlattenedBlock, List<FlattenedTransactionTrace>)> FlattenAsync(Block block)
    {
        return Task.Run(() =>
        {
            var flattenedTransactionTraces = block.UnfilteredTransactionTraces.Select(transactionTrace =>
                new FlattenedTransactionTrace
                {
                    BlockNum = block.Number,
                    DbOps = transactionTrace.DbOps.ToArray(),
                    Elapsed = transactionTrace.Elapsed,
                    Id = transactionTrace.Id,
                    NetUsage = transactionTrace.NetUsage,
                    TableOps = transactionTrace.TableOps.ToArray(),
                    ActionTraces = transactionTrace.ActionTraces.Where(_actionFilter).Select((actionTrace, actionIndex) =>
                        new FlattenedActionTrace()
                        {
                            AccountRamDeltas = actionTrace.AccountRamDeltas,
                            Act = actionTrace.Act,
                            Console = actionTrace.Console,
                            ContextFree = actionTrace.ContextFree,
                            DbOps = transactionTrace.DbOps.Where(dbOp => _deltaFilter(dbOp) && dbOp.ActionIndex == actionIndex).Select(dbOp =>
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
                            RamOps = transactionTrace.RamOps.Where(ramOp => ramOp.ActionIndex == actionIndex).Select(
                                ramOp =>
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
                            TableOps = transactionTrace.TableOps.Where(tableOp => tableOp.ActionIndex == actionIndex)
                                .Select(
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
                }).Where(_filterEmptyTransactionsFilter).ToList();

            return (new FlattenedBlock()
            {
                Number = block.Number,
                TransactionIds = flattenedTransactionTraces.Select(ut => ut.Id).ToArray(),
                Id = block.Id,
                Producer = block.Header.Producer,
                ProducerSignature = block.ProducerSignature
            }, flattenedTransactionTraces);
        });
    }

    private async Task TestAbiDeserializer(List<FlattenedTransactionTrace> flattenedTransactionTraces)
    {
        foreach(FlattenedTransactionTrace trace in flattenedTransactionTraces)
        {
            foreach(var actionTrace in trace.ActionTraces)
            {
                try
                {
                    var (found, assemblyPair) = await _storageAdapter.TryGetActiveAbiAssembly(actionTrace.Act.Account);
                    if (found)  // TODO check GlobalSequence
                    {
                        var assembly = assemblyPair.Value;
                        var clrType = assembly.GetType(actionTrace.Act.Name.StringVal);
                        if (clrType != null)
                        {
                            BinaryReader reader = new BinaryReader(new MemoryStream(actionTrace.Act.Data));
                            var obj = clrType.GetConstructor(new Type[] { typeof(BinaryReader) })!.Invoke(new[] { reader });
                        }
                        else
                            Log.Information($"Type for {actionTrace.Act.Account}.{actionTrace.Act.Name} not found");
                    }
                }
                catch(Exception ex)
                {
                    Log.Error(ex, "");
                }
            }
        }
    }

}