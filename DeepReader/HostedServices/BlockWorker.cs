using DeepReader.Classes;
using DeepReader.Options;
using DeepReader.Storage;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Other;
using KGySoft.CoreLibraries;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Prometheus;
using Serilog;
using System.Diagnostics;
using System.Threading.Channels;
using DeepReader.Types.Enums;

namespace DeepReader.HostedServices;

public class BlockWorker : BackgroundService
{
    private readonly ChannelReader<Block> _blocksChannel;
    private readonly ChannelReader<List<IList<StringSegment>>> _blockSegmentsListChannel;

    private readonly IStorageAdapter _storageAdapter;

    private MindReaderOptions _mindReaderOptions;
    private DeepReaderOptions _deepReaderOptions;

    private readonly ParallelOptions _postProcessingParallelOptions;

    private static readonly Histogram BlocksChannelSize = Metrics.CreateHistogram("deepreader_blockworker_block_channel_size", "The current size of the channel size in block worker");
    private static readonly Histogram BlockSegmentListChannelSize = Metrics.CreateHistogram("deepreader_blockworker_segment_list_channel_size", "The current size of the channel size in block worker");

    private readonly Func<Types.StorageTypes.TransactionTrace, bool> _filterEmptyTransactionsFilter;

    private readonly Func<ActionTrace, bool> _actionFilter;
    private readonly Func<ExtendedDbOp, bool> _deltaFilter;

    private readonly ObjectPool<Block> _blockPool;

    // used to decode ABIs

    private readonly Name _eosio = NameCache.GetOrCreate("eosio");
    private readonly Name _setabi = NameCache.GetOrCreate("setabi");
    private readonly AbiDecoder _abiDecoder;

    private MetricsCollector _metricsCollector;

    public BlockWorker(
        ChannelReader<Block> blocksChannel,
        ChannelReader<List<IList<StringSegment>>> blockSegmentsListChannel,
        IStorageAdapter storageAdapter,
        IOptionsMonitor<MindReaderOptions> mindReaderOptionsMonitor,
        IOptionsMonitor<DeepReaderOptions> deepReaderOptionsMonitor,
        ObjectPool<Block> blockPool,
        MetricsCollector metricsCollector)
    {
        _mindReaderOptions = mindReaderOptionsMonitor.CurrentValue;
        mindReaderOptionsMonitor.OnChange(OnMindReaderOptionsChanged);

        _deepReaderOptions = deepReaderOptionsMonitor.CurrentValue;
        deepReaderOptionsMonitor.OnChange(OnDeepReaderOptionsChanged);

        _postProcessingParallelOptions = new ParallelOptions()
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
        _metricsCollector = metricsCollector;
        _metricsCollector.CollectMetricsHandler += CollectObservableMetrics;

        _abiDecoder = new AbiDecoder(_storageAdapter);
    }

    private void CollectObservableMetrics(object? sender, EventArgs e)
    {
        if (_blocksChannel.CanCount)
            BlocksChannelSize.Observe(_blocksChannel.Count);
        if (_blockSegmentsListChannel.CanCount)
            BlockSegmentListChannelSize.Observe(_blockSegmentsListChannel.Count);
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
        var blockProcessingTasks = new List<Task>();
        for (var i = 0; i < _deepReaderOptions.BlockProcessingTasks; i++)
        {
            blockProcessingTasks.Add(ProcessBlocks(stoppingToken));
        }
        await Task.WhenAll(blockProcessingTasks);
    }

    private async Task ProcessBlocks(CancellationToken cancellationToken)
    {
        uint blockNum = 0;
        Types.StorageTypes.Block block = new Types.StorageTypes.Block();
        List<Types.StorageTypes.TransactionTrace> transactionTraces = new List<Types.StorageTypes.TransactionTrace>();
        List<Types.StorageTypes.ActionTrace> actionTraces = new List<Types.StorageTypes.ActionTrace>();

        await foreach (var deepMindBlock in _blocksChannel.ReadAllAsync(cancellationToken))
        {
            try
            {
                if (deepMindBlock.Number % 1000 == 0)
                {
                    if (_blocksChannel.CanCount)
                        Log.Information($"blocks-channel-size: {_blocksChannel.Count}");

                    if (_blockSegmentsListChannel.CanCount)
                        Log.Information($"segment-channel-size: {_blockSegmentsListChannel.Count}");

                    Log.Information($"got block {deepMindBlock.Number}");
                    Log.Information($"Current Threads: {Process.GetCurrentProcess().Threads.Count}");
                }

                block = Types.StorageTypes.Block.FromPool();

                actionTraces.Clear();
                transactionTraces.Clear();

                PostProcess(deepMindBlock, block, actionTraces);

                // need to copy the transactionsList here as when a block gets evicted, the list gets cleared
                // (theoretically possible before call finishes - but very unlikely)
                transactionTraces.AddRange(block.Transactions);
                actionTraces = actionTraces.Where(a => a.Receipt != null).ToList();

                // this (currently) needs to be done before storage as eviction
                // will put actionTraces and childs back into pools
                // (theoretically possible before call finishes - but very unlikely)
                await CheckForAbiUpdates(actionTraces);

                await Task.WhenAll(
                    _storageAdapter.StoreBlockAsync(block),

                    StoreTransactionTraces(transactionTraces),

                    StoreActionTraces(actionTraces)
                );
            }
            catch (Exception e)
            {
                Log.Error(e, " at block {@blockNum}", block.Number);

                // in case of failure, return all objects recursively to their pools
                block.ReturnToPoolRecursive();

                foreach (var transactionTrace in transactionTraces)
                    transactionTrace.ReturnToPoolRecursive();
                
                foreach (var actionTrace in actionTraces)
                    actionTrace.ReturnToPoolRecursive();
            }
            finally
            {
                _blockPool.Return(deepMindBlock);
            }
        }
    }

    private async Task StoreTransactionTraces(IEnumerable<Types.StorageTypes.TransactionTrace> transactionTraces)
    {
        foreach (var transactionTrace in transactionTraces)
        {
            await _storageAdapter.StoreTransactionAsync(transactionTrace);
        }
    }

    private async Task StoreActionTraces(List<Types.StorageTypes.ActionTrace> actionTraces)
    {
        foreach (var actionTrace in actionTraces)
        {
            await _storageAdapter.StoreActionTraceAsync(actionTrace);
        }
    }

    private Types.StorageTypes.ActionTrace ProcessChildActions(CreationTreeNode creationTreeNode, TransactionTrace trx, Types.StorageTypes.ActionTrace creatorAction, List<Types.StorageTypes.ActionTrace> allActions)
    {
        var childActionTrace = Types.StorageTypes.ActionTrace.FromPool(); // returned to the Pool when Faster evicts it
        childActionTrace.CopyFrom(trx.ActionTraces[creationTreeNode.ActionIndex]);

        if (creationTreeNode.Kind is CreationOpKind.INLINE or CreationOpKind.NOTIFY or CreationOpKind.CFA_INLINE)
        {
            allActions.Add(childActionTrace);
            var childCreatedActionTraces = new List<Types.StorageTypes.ActionTrace>();
            foreach (var creationTreeChildChild in creationTreeNode.Children)
            {
                childCreatedActionTraces.Add(ProcessChildActions(creationTreeChildChild, trx, childActionTrace, allActions));
            }

            if (creationTreeNode.Kind == CreationOpKind.NOTIFY)
                childActionTrace.IsNotify = true;
            
            childActionTrace.CreatorActionId = creatorAction.Receipt.GlobalSequence;

            childActionTrace.CreatedActions = childCreatedActionTraces.ToArray();
            childActionTrace.CreatedActionIds = childCreatedActionTraces.Select(a => a.Receipt.GlobalSequence).ToArray();
            
            childActionTrace.DbOps = trx.DbOps.Where(dbop => dbop.ActionIndex == creationTreeNode.ActionIndex).Cast<DbOp>().ToArray();
            childActionTrace.RamOps = trx.RamOps.Where(ramop => ramop.ActionIndex == creationTreeNode.ActionIndex).Cast<RamOp>().ToArray();
            childActionTrace.TableOps = trx.TableOps.Where(tableop => tableop.ActionIndex == creationTreeNode.ActionIndex).Cast<TableOp>().ToArray();
        }
        else
            Log.Error($"CreationTreeChild-Issues in trx {trx.Id.StringVal}");

        return childActionTrace;

    }

    private void PostProcess(Block deepMindBlock, Types.StorageTypes.Block block, List<Types.StorageTypes.ActionTrace> actionTraces)
    {
        for (var transactionTraceIndex = 0; transactionTraceIndex < deepMindBlock.UnfilteredTransactionTraces.Count; transactionTraceIndex++)
        {
            var trx = deepMindBlock.UnfilteredTransactionTraces[transactionTraceIndex];

            var transactionTrace = Types.StorageTypes.TransactionTrace.FromPool(); // returned to the Pool when Faster evicts it
            transactionTrace.CopyFrom(trx);

            if (!trx.DtrxOps.Any(dtrxOp => dtrxOp.Operation == DTrxOpOperation.FAILED))
            {
                var rootActionTraces = new Types.StorageTypes.ActionTrace[trx.CreationTreeRoots.Count];

                for (var creationTreeRootIndex = 0;
                     creationTreeRootIndex < trx.CreationTreeRoots.Count;
                     creationTreeRootIndex++)
                {
                    var creationTreeRoot = trx.CreationTreeRoots[creationTreeRootIndex];
                    ProcessCreationTreeRoot(creationTreeRoot, creationTreeRootIndex, trx, rootActionTraces,
                        actionTraces);
                }

                rootActionTraces = rootActionTraces.Where(a => a.Receipt != null).ToArray();
                transactionTrace.ActionTraces = rootActionTraces;
                transactionTrace.ActionTraceIds = rootActionTraces.Select(a => a.Receipt.GlobalSequence).ToArray();
            }
            else
            {
                Log.Information("Found failed Deferred Transaction");
            }

            block.Transactions.Add(transactionTrace);
            block.TransactionIds.Add(transactionTrace.Id);

        }


        //Parallel.ForEach(block.UnfilteredTransactionTraces, _postProcessingParallelOptions,
        //    (trx, _, index) =>
        //    {
        //        //trx.ActionTraces = trx.ActionTraces.Where(_actionFilter).ToArray();// Filter AcionTraces

        //        var rootActionTraces = new Types.StorageTypes.ActionTrace[trx.CreationTreeRoots.Count];
        //        Parallel.ForEach(trx.CreationTreeRoots, _postProcessingParallelOptions, (creationTreeRoot, _, creationTreeRootIndex) => ProcessCreationTreeRoot(creationTreeRoot, _, creationTreeRootIndex, trx, rootActionTraces, actionTraces));
        //        var transactionTrace = new Types.StorageTypes.TransactionTrace(trx)
        //        {
        //            ActionTraces = rootActionTraces.ToArray(),
        //            ActionTraceIds = rootActionTraces.Select(a => a.Receipt.GlobalSequence).ToArray()
        //        };

        //        while (!actionTraces.TryAddRange(rootActionTraces))
        //        {
        //            if (actionTraces.TryAddRange(rootActionTraces))
        //                break;
        //        }

        //        transactionTraces[index] = transactionTrace;
        //    });

        block.Transactions = block.Transactions.Where(_filterEmptyTransactionsFilter).ToList();

        block.CopyFrom(deepMindBlock);
    }

    private void ProcessCreationTreeRoot(CreationTreeNode creationTreeRoot, int creationTreeRootIndex, TransactionTrace trx, Types.StorageTypes.ActionTrace[] rootActionTraces, List<Types.StorageTypes.ActionTrace> actionTraces)
    {
        if (creationTreeRoot.Kind == CreationOpKind.ROOT)
        {
            var rootAction = Types.StorageTypes.ActionTrace.FromPool(); // returned to the Pool when Faster evicts it
            rootAction.CopyFrom(trx.ActionTraces[creationTreeRoot.ActionIndex]);

            var createdActionTraces = new List<Types.StorageTypes.ActionTrace>();
            rootActionTraces[creationTreeRootIndex] = rootAction;
            actionTraces.Add(rootAction);
            foreach (var creationTreeChild in creationTreeRoot.Children)
            {
                if (creationTreeChild.Kind is CreationOpKind.INLINE or CreationOpKind.NOTIFY or CreationOpKind.CFA_INLINE)
                {
                    createdActionTraces.Add(ProcessChildActions(creationTreeChild, trx, rootAction, actionTraces));
                }
                else
                    Log.Error($"CreationTreeChild-Issues in trx {trx.Id.StringVal}");
            }

            rootAction.CreatedActions = createdActionTraces.ToArray();
            rootAction.CreatedActionIds = createdActionTraces.Select(a => a.Receipt.GlobalSequence).ToArray();

            rootAction.DbOps = trx.DbOps.Where(dbop => dbop.ActionIndex == creationTreeRoot.ActionIndex).Cast<DbOp>().ToArray();
            rootAction.RamOps = trx.RamOps.Where(ramop => ramop.ActionIndex == creationTreeRoot.ActionIndex).Cast<RamOp>().ToArray();
            rootAction.TableOps = trx.TableOps.Where(tableop => tableop.ActionIndex == creationTreeRoot.ActionIndex).Cast<TableOp>().ToArray();
        }
        else
            Log.Error($"CreationTreeRoot-Issues in trx {trx.Id.StringVal}");
    }

    //private async Task TestAbiDeserializer(List<Types.StorageTypes.TransactionTrace> flattenedTransactionTraces)
    //{
    //    foreach(var trace in flattenedTransactionTraces)
    //    {
    //        foreach(var actionTrace in trace.ActionTraces)
    //        {
    //            string clrTypename = "";
    //            string actName = "";
    //            try
    //            {
    //                var (found, assemblyPair) = await _storageAdapter.TryGetActiveAbiAssembly(actionTrace.Act.Account);
    //                if (found)  // TODO check GlobalSequence
    //                {
    //                    actName = actionTrace.Act.Name;
    //                    var assembly = assemblyPair.Value;
    //                    var clrType = assembly.Assembly.GetType($"_{actionTrace.Act.Name.StringVal.Replace('.','_')}");
    //                    if (clrType != null)
    //                    {
    //                        BinaryReader reader = new BinaryReader(new MemoryStream(actionTrace.Act.Data));

    //                        clrTypename = clrType.Name;
    //                        var obj = Activator.CreateInstance(clrType, reader);

    //                        //Log.Information(JsonSerializer.Serialize(obj, new JsonSerializerOptions() { IncludeFields = true, WriteIndented = true }));
    //                        //var ctor = clrType.GetConstructor(new Type[] { typeof(BinaryReader) });
    //                        //ctor.Invoke(new[] { reader });

    //                        if (actionTrace.Act.Account == _eosio && actionTrace.Act.Name == _setabi)
    //                        {
    //                            var abiField = clrType.GetField("_abi");
    //                            var abiFieldValue = abiField.GetValue(obj);
    //                            var abiBytes = (byte[])abiFieldValue;

    //                            var accountField = clrType.GetField("_account");
    //                            var accountFieldValue = accountField.GetValue(obj);
    //                            var account = (Name)accountFieldValue;

    //                            _abiDecoder.AddAbi(account, abiBytes, actionTrace.GlobalSequence);
    //                        }
    //                    }
    //                    else
    //                        Log.Information($"Type for {actionTrace.Act.Account.StringVal}.{actionTrace.Act.Name.StringVal} not found");
    //                }
    //            }
    //            catch(Exception ex)
    //            {
    //                Log.Error(ex, "");
    //                Log.Information(clrTypename); 
    //                Log.Information(actName);
    //            }
    //        }
    //    }
    //}

    private static readonly RecyclableMemoryStreamManager StreamManager = new();

    private async Task CheckForAbiUpdates(IEnumerable<Types.StorageTypes.ActionTrace> actionTraces)
    {
        await Parallel.ForEachAsync(actionTraces.Where(at => at.Act.Account == _eosio && at.Act.Name == _setabi), _postProcessingParallelOptions, async (setAbiTrace, _) =>
        {
            try
            {
                var (found, assemblyPair) = await _storageAdapter.TryGetActiveAbiAssembly(_eosio); // account is always eosio here
                if (found)  // TODO check GlobalSequence validity
                {
                    var clrType = assemblyPair.Value.Assembly.GetType($"_setabi"); // type is always setabi here
                    if (clrType != null)
                    {                
                        using var stream = StreamManager.GetStream(setAbiTrace.Act.Data);// copies buffer/bytes
                        using var reader = new BinaryReader(stream);

                        var obj = Activator.CreateInstance(clrType, reader);

                        var abiField = clrType.GetField("_abi")!;
                        var abiFieldValue = abiField.GetValue(obj)!;
                        var abiBytes = (byte[])abiFieldValue;

                        var accountField = clrType.GetField("_account")!;
                        var accountFieldValue = accountField.GetValue(obj)!;
                        var account = (Name)accountFieldValue;

                        _abiDecoder.AddAbi(account, abiBytes, setAbiTrace.GlobalSequence);
                    }
                    else
                        Log.Information($"Type for {setAbiTrace.Act.Account.StringVal}.{setAbiTrace.Act.Name.StringVal} not found");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
            }
        });
    }

}