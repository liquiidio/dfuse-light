using DeepReader;
using DeepReader.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddSingleton(Channel.CreateUnbounded<Block>(new UnboundedChannelOptions() { SingleReader = false, SingleWriter = true }));
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Reader);
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Writer);
        services.AddHostedService<Worker>();
        services.AddHostedService<BlockWorker>();
    })
    .Build();

await host.RunAsync();