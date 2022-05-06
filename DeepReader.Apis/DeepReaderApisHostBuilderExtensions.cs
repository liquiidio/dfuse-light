using System.Text.Json;
using DeepReader.Apis.GraphQl.QueryTypes;
using DeepReader.Apis.GraphQl.SubscriptionTypes;
using DeepReader.Apis.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace DeepReader.Apis
{
    public static class DeepReaderApisHostBuilderExtensions
    {
        public static IHostBuilder UseDeepReaderApis(
           this IHostBuilder builder,
           ILogger logger = null,
           bool dispose = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices((hostContext, services) =>
                {
                    services.Configure<ApiOptions>(config => hostContext.Configuration.GetSection("ApiOptions").Bind(config));
                    services.AddControllers().AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.MaxDepth = Int32.MaxValue;
                        options.JsonSerializerOptions.IncludeFields = true;
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    });
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen();

                    services
                        .AddGraphQLServer()
                        .AddInMemorySubscriptions()
                        .AddQueryType(q => q.Name("Query"))
                            .AddType<BlockQueryType>()
                            .AddType<TransactionQueryType>()
                        .AddSubscriptionType(s => s.Name("Subscription"))
                            .AddType<BlockSubscriptionType>()
                            .AddType<TransactionSubscriptionType>();
                    services.AddSentry();
                });
                webBuilder.Configure((context, app) =>
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DeepReaderAPI v1");
                    });
                    //app.UseHttpsRedirection();

                    app.UseWebSockets();
                    app.UseRouting();

                    // Expose Metrics only on specified port (default 9090)
                    // so they are not proxied with the api and only used by internal services

                    // TODO @Haron I would like to have port and url configurable but this doesn't seem to work
                    // app.UseMetricServer(context.Configuration.Get<ApiOptions>().MetricsPort, context.Configuration.Get<ApiOptions>().MetricsUrl);
                    
                    // Exposes HTTP metrics to Prometheus using the same endpoint above
                    app.UseHttpMetrics(options =>
                    {
                        // This will preserve only the first digit of the status code.
                        // For example: 200, 201, 203 -> 2xx
                        options.ReduceStatusCodeCardinality();
                    });

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGraphQL();
                        endpoints.MapControllers();
                        endpoints.MapMetrics();
                    });

                    app.UseSentryTracing();
                });
            });
            return builder;
        }
    }
}