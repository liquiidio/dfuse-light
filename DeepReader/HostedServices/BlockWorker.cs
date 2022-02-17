using System.Text.Json;
using System.Threading.Channels;
using DeepReader.Storage;
using DeepReader.Types;
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

            // TODO PostProcess (flatten Blocks and Traces) and store them
        }
    }
}