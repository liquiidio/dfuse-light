using System.Threading.Channels;
using DeepReader.Apis.GraphQl;
using DeepReader.Apis.REST;
using DeepReader.Configuration;
using DeepReader.HostedServices;
using DeepReader.Options;
using DeepReader.Storage.Faster;
using DeepReader.Types;
using KGySoft.CoreLibraries;
using Serilog;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<DeepReaderOptions>(config => hostContext.Configuration.GetSection("DeepReaderOptions").Bind(config));
        services.Configure<MindReaderOptions>(config => hostContext.Configuration.GetSection("MindReaderOptions").Bind(config));

        services.AddSingleton(Channel.CreateBounded<IList<IList<StringSegment>>>(new BoundedChannelOptions(50000) { SingleReader = false, SingleWriter = true, FullMode = BoundedChannelFullMode.Wait }));
        services.AddSingleton(svc => svc.GetRequiredService<Channel<IList<IList<StringSegment>>>>().Reader);
        services.AddSingleton(svc => svc.GetRequiredService<Channel<IList<IList<StringSegment>>>>().Writer);
        
        services.AddSingleton(Channel.CreateUnbounded<Block>(new UnboundedChannelOptions() { SingleReader = false, SingleWriter = false }));
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Reader);
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Writer);

        services.AddHostedService<DlogReaderWorker>();
        services.AddHostedService<DlogParserWorker>();
        services.AddHostedService<BlockWorker>();

    }).UseSerilog((hostingContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
    )
    .UseDeepReaderGraphQl()

    .UseDeepReaderRest()
    .UseFasterStorage()
    .Build();

await host.RunAsync();
