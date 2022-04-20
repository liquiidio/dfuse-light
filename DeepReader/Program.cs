using System.Threading.Channels;
using DeepReader.Apis.GraphQl;
using DeepReader.Apis.Options;
using DeepReader.Apis.REST;
using DeepReader.Configuration;
using DeepReader.HostedServices;
using DeepReader.Options;
using DeepReader.Storage.Elastic;
using DeepReader.Storage.Faster;
using DeepReader.Storage.Options;
using DeepReader.Types;
using Serilog;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<DeepReaderOptions>(config => hostContext.Configuration.GetSection("DeepReaderOptions").Bind(config));
        services.Configure<MindReaderOptions>(config => hostContext.Configuration.GetSection("MindReaderOptions").Bind(config));
        services.AddSingleton(Channel.CreateUnbounded<Block>(new UnboundedChannelOptions() { SingleReader = false, SingleWriter = true }));
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Reader);
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Writer);
        services.AddHostedService<DlogReaderWorker>();
        services.AddHostedService<BlockWorker>();
    }).UseSerilog((hostingContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
    )
    .UseDeepReaderGraphQl()
    .UseDeepReaderRest()
    .UseFasterStorage()
    .Build();

await host.RunAsync();
