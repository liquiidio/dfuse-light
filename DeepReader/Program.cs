using DeepReader;
using DeepReader.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;
using Serilog;



IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddSingleton(Channel.CreateUnbounded<Block>(new UnboundedChannelOptions() { SingleReader = false, SingleWriter = true }));
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Reader);
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Writer);
        services.AddHostedService<DlogReaderWorker>();
        services.AddHostedService<BlockWorker>();
    }).UseSerilog((hostingContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
    )
    //.UseSerilog((ctx, lc) => lc
    //.WriteTo.Console()
    //.WriteTo.Seq("http://localhost:5341"));
    .Build();

await host.RunAsync();