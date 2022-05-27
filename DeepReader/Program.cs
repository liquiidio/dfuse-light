using System.Threading.Channels;
using DeepReader.Apis;
using DeepReader.HostedServices;
using DeepReader.Options;
using DeepReader.Pools;
using DeepReader.Storage.Faster;
using DeepReader.Types;
using KGySoft.CoreLibraries;
using Microsoft.Extensions.ObjectPool;
using Serilog;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Configure and bind Options
        services.Configure<DeepReaderOptions>(config =>
            hostContext.Configuration.GetSection("DeepReaderOptions").Bind(config));
        services.Configure<MindReaderOptions>(config =>
            hostContext.Configuration.GetSection("MindReaderOptions").Bind(config));

        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<ObjectPoolProvider>()
            .Create(new BlockSegmentListPooledObjectPolicy(Convert.ToInt32(hostContext.Configuration.GetSection("DeepReaderOptions")["DlogBlockSegmentListSize"]))));
        services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<ObjectPoolProvider>()
            .Create(new BlockPooledObjectPolicy()));
        services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<ObjectPoolProvider>()
            .Create(new ActionTraceListPooledObjectPolicy()));

        //services.AddSingleton(Channel.CreateBounded<List<IList<StringSegment>>>(
        //    new BoundedChannelOptions(
        //        Convert.ToInt32(hostContext.Configuration.GetSection("DeepReaderOptions")["DlogReaderBlockQueueSize"]))
        //    {
        //        SingleReader = false, SingleWriter = true, FullMode = BoundedChannelFullMode.Wait
        //    }));

        // Inject BlockSegment-Channel
        services.AddSingleton(
            Channel.CreateUnbounded<List<IList<StringSegment>>>(
                new UnboundedChannelOptions() {SingleReader = false, SingleWriter = true}));
        services.AddSingleton(svc => svc.GetRequiredService<Channel<List<IList<StringSegment>>>>().Reader);
        services.AddSingleton(svc => svc.GetRequiredService<Channel<List<IList<StringSegment>>>>().Writer);

        services.AddSentry();

        // Inject Block-Channel
        services.AddSingleton(
            Channel.CreateUnbounded<Block>(
                new UnboundedChannelOptions() {SingleReader = false, SingleWriter = false}));
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Reader);
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Writer);

        // Inject Workers
        services.AddHostedService<DlogReaderWorker>();
        services.AddHostedService<DlogParserWorker>();
        services.AddHostedService<BlockWorker>();


    }).UseSerilog((hostingContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
    )
    .UseDeepReaderApis()
    .UseFasterStorage()
#if !DEBUG
    .UseSystemd()
#endif
    .Build();

await host.RunAsync();
