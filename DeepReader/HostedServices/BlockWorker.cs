using System.Text.Json;
using System.Threading.Channels;
using DeepReader.AssemblyGenerator;
using DeepReader.DeepMindDeserializer;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using Serilog;

namespace DeepReader.HostedServices;

public class BlockWorker : BackgroundService
{
    private readonly ChannelReader<Block> _blocksChannel;

    public BlockWorker(ChannelReader<Block> blocksChannel)
    {
        _blocksChannel = blocksChannel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ProcessBlocks(stoppingToken);
    }

    private async Task ProcessBlocks(CancellationToken clt)
    {
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            IncludeFields = true,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            MaxDepth = 0
        };

        await foreach (var block in _blocksChannel.ReadAllAsync(clt))
        {
            if(block.Number % 1000 == 0)
                Log.Information($"got block {block.Number}");

            foreach (var setAbiAction in block.UnfilteredTransactionTraces.SelectMany(utt => utt.ActionTraces.Where(at => at.Act.Account == "eosio" && at.Act.Name == "setabi")))
            {
                Log.Information($"got abi for {setAbiAction.Act.Account} at {block.Number}");

                var abi = Deserializer.Deserialize<Abi>(setAbiAction.Act.Data);
                if (abi != null && abi.AbiActions.Length > 0 || abi.AbiStructs.Length > 0)
                {
                    Log.Information(JsonSerializer.Serialize(abi, jsonSerializerOptions));
                }
            }

            //foreach (var unfilteredTransactionTrace in block.UnfilteredTransactionTraces)
            //{
            //    // FlattenedTransactionTrace
            //    //      FlattenedActionTraces (containing the diverse Ops)
            //    // removing 
            //    // Key = TransactionId
            //}

        }
    }
}