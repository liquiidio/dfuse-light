using DeepReader.Types;
using System.Threading.Channels;
using DeepReader.HostedServices;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddSingleton(Channel.CreateUnbounded<Block>(new UnboundedChannelOptions() { SingleReader = false, SingleWriter = true }));
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Reader);
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Writer);
        services.AddHostedService<DlogReaderWorker>();
        services.AddHostedService<BlockWorker>();
    }).UseSerilog((hostingContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
    )
    .Build();

await host.RunAsync();

// Notes:
// interesting thread on AutoRegisteringGraphTypes and other stuff with graphql-dotnet https://github.com/graphql-dotnet/graphql-dotnet/issues/576