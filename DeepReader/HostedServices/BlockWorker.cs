using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using DeepReader.Options;
using DeepReader.Storage;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
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

    private readonly ParallelOptions _postProcessingParallelOptions;

    private static readonly Histogram BlocksChannelSize = Metrics.CreateHistogram("deepreader_blockworker_block_channel_size", "The current size of the channel size in block worker");

    private readonly Func<Types.StorageTypes.TransactionTrace, bool> _filterEmptyTransactionsFilter;

    private readonly Func<ActionTrace, bool> _actionFilter;
    private readonly Func<ExtendedDbOp, bool> _deltaFilter;

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

                var (block, transactionTraces, actionTraces) = await PostProcessAsync(deepMindBlock);

                await Task.WhenAll(

                    _storageAdapter.StoreBlockAsync(block),

                    Parallel.ForEachAsync(transactionTraces, _postProcessingParallelOptions,
                        async (transactionTrace, _) =>
                        {
                            await _storageAdapter.StoreTransactionAsync(
                                transactionTrace); // TODO cancellationToken
                        }),

                    Parallel.ForEachAsync(actionTraces, _postProcessingParallelOptions,
                        async (actionTrace, _) =>
                        {
                            await _storageAdapter.StoreActionTraceAsync(actionTrace); // TODO cancellationToken
                        })
                );
            }
            catch (Exception e)
            {
                Log.Error(e, "");
            }
            finally
            {
                _blockPool.Return(deepMindBlock);
            }
        }
    }

    Types.StorageTypes.ActionTrace ProcessChildActions(CreationTreeNode creationTreeNode, TransactionTrace trx, Types.StorageTypes.ActionTrace creatorAction, BlockingCollection<Types.StorageTypes.ActionTrace> allActions)
    {
        var createdActionTraces = new List<Types.StorageTypes.ActionTrace>();
        var childActionTrace = new Types.StorageTypes.ActionTrace(trx.ActionTraces[creationTreeNode.ActionIndex]);
        if (creationTreeNode.Kind == CreationOpKind.INLINE || creationTreeNode.Kind == CreationOpKind.NOTIFY || creationTreeNode.Kind == CreationOpKind.CFA_INLINE)
        {
            createdActionTraces.Add(childActionTrace);
            var childCreatedActionTraces = new List<Types.StorageTypes.ActionTrace>();
            foreach (var creationTreeChildChild in creationTreeNode.Children)
            {
                childCreatedActionTraces.Add(ProcessChildActions(creationTreeChildChild, trx, childActionTrace, allActions));
            }

            if (creationTreeNode.Kind == CreationOpKind.NOTIFY)
                childActionTrace.IsNotify = true;
            
            childActionTrace.CreatorAction = creatorAction;
            childActionTrace.CreatorActionId = creatorAction.Receipt.GlobalSequence;

            childActionTrace.CreatedActions = childCreatedActionTraces.ToArray();
            childActionTrace.CreatedActionIds = childCreatedActionTraces.Select(a => a.Receipt.GlobalSequence).ToArray();
            
            childActionTrace.DbOps = trx.DbOps.Where(dbop => dbop.ActionIndex == creationTreeNode.ActionIndex).ToArray();
            childActionTrace.RamOps = trx.RamOps.Where(ramop => ramop.ActionIndex == creationTreeNode.ActionIndex).ToArray();
            childActionTrace.TableOps = trx.TableOps.Where(tableop => tableop.ActionIndex == creationTreeNode.ActionIndex).ToArray();
        }
        else
            Log.Error($"CreationTreeChild-Issues in trx {trx.Id.StringVal}");

        while (!allActions.TryAddRange(createdActionTraces))
        {
            if (allActions.TryAddRange(createdActionTraces))
                break;
        }
        return childActionTrace;

    }

    private Task<(Types.StorageTypes.Block, Types.StorageTypes.TransactionTrace[], List<Types.StorageTypes.ActionTrace>)> PostProcessAsync(Block block)
    {
        return Task.Run(() =>
        {
            var transactionTraces = new Types.StorageTypes.TransactionTrace[block.UnfilteredTransactionTraces.Count];
            var actionTraces = new BlockingCollection<Types.StorageTypes.ActionTrace>();
            
            var res = Parallel.ForEach(block.UnfilteredTransactionTraces, _postProcessingParallelOptions,
                (trx, _, index) =>
                {
                    trx.ActionTraces = trx.ActionTraces.Where(_actionFilter).ToArray();// Filter AcionTraces

                    var rootActionTraces = new List<Types.StorageTypes.ActionTrace>();
                    foreach (var creationTreeRoot in trx.CreationTreeRoots)
                    {
                        if(creationTreeRoot.Kind == CreationOpKind.ROOT)
                        {
                            var rootAction = new Types.StorageTypes.ActionTrace(trx.ActionTraces[creationTreeRoot.ActionIndex]);
                            var createdActionTraces = new List<Types.StorageTypes.ActionTrace>();
                            rootActionTraces.Add(rootAction);
                            foreach(var creationTreeChild in creationTreeRoot.Children)
                            {
                                if(creationTreeChild.Kind == CreationOpKind.INLINE || creationTreeChild.Kind == CreationOpKind.NOTIFY || creationTreeChild.Kind == CreationOpKind.CFA_INLINE)
                                {
                                    createdActionTraces.Add(ProcessChildActions(creationTreeChild, trx, rootAction, actionTraces));
                                }
                                else
                                    Log.Error($"CreationTreeChild-Issues in trx {trx.Id.StringVal}");
                            }

                            rootAction.CreatedActions = createdActionTraces.ToArray();
                            rootAction.CreatedActionIds = createdActionTraces.Select(a => a.Receipt.GlobalSequence).ToArray();

                            rootAction.DbOps = trx.DbOps.Where(dbop => dbop.ActionIndex == creationTreeRoot.ActionIndex).ToArray();
                            rootAction.RamOps = trx.RamOps.Where(ramop => ramop.ActionIndex == creationTreeRoot.ActionIndex).ToArray();
                            rootAction.TableOps = trx.TableOps.Where(tableop => tableop.ActionIndex == creationTreeRoot.ActionIndex).ToArray();
                        }
                        else
                            Log.Error($"CreationTreeRoot-Issues in trx {trx.Id.StringVal}");
                    }
                    var transactionTrace = new Types.StorageTypes.TransactionTrace(trx)
                    {
                        ActionTraces = rootActionTraces.ToArray(),
                        ActionTraceIds = rootActionTraces.Select(a => a.Receipt.GlobalSequence).ToArray()
                    };

                    while (!actionTraces.TryAddRange(rootActionTraces))
                    {
                        if (actionTraces.TryAddRange(rootActionTraces))
                            break;
                    }

                    transactionTraces[index] = transactionTrace;
                });

            transactionTraces = transactionTraces.Where(_filterEmptyTransactionsFilter).ToArray();

            return (new Types.StorageTypes.Block()
            {
                Id = block.Id,
                Number = block.Number,
                Timestamp = block.Header.Timestamp,
                ActionMroot = block.Header.ActionMroot,
                Confirmed = block.Header.Confirmed,
                Previous = block.Header.Previous,
                NewProducers = block.Header.NewProducers,
                ScheduleVersion = block.Header.ScheduleVersion,
                TransactionMroot = block.Header.TransactionMroot,
                Transactions = transactionTraces,
                TransactionIds = transactionTraces.Select(ut => ut.Id).ToArray(),
                Producer = block.Header.Producer,
                ProducerSignature = block.ProducerSignature
            }, transactionTraces, actionTraces.ToList());
        });
    }

}