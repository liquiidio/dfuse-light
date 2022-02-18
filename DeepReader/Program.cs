using System.Threading.Channels;
using DeepReader.Apis.GraphQl;
using DeepReader.HostedServices;
using DeepReader.Storage.Elastic;
using DeepReader.Storage.Faster;
using DeepReader.Types;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    //.ConfigureWebHost(builder => builder.UseStartup<Startup>())
    .ConfigureServices(services => {
        services.AddSingleton(Channel.CreateUnbounded<Block>(new UnboundedChannelOptions() { SingleReader = false, SingleWriter = true }));
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Reader);
        services.AddSingleton(svc => svc.GetRequiredService<Channel<Block>>().Writer);
        services.AddHostedService<DlogReaderWorker>();
        services.AddHostedService<BlockWorker>();
    }).UseSerilog((hostingContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
    )
    .UseDeepReaderGraphQl()
//    .UseElasticStorage()
    .UseFasterStorage()
    .Build();

await host.RunAsync();


//internal class Startup
//{
//    public Startup(IConfiguration configuration)
//    {
//        Configuration = configuration;
//    }

//    public IConfiguration Configuration { get; }

//    // Use this method to add services to the container.  
//    public void ConfigureServices(IServiceCollection services)
//    {
//        services.AddGraphQL();
//    }
//    // Use this method to configure the HTTP request pipeline.  
//    public void Configure(IApplicationBuilder app)
//    {
//    }
//}
// Notes:
// interesting thread on AutoRegisteringGraphTypes and other stuff with graphql-dotnet https://github.com/graphql-dotnet/graphql-dotnet/issues/576