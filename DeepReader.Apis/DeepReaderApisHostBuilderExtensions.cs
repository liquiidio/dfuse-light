
﻿using DeepReader.Apis.GraphQl.QueryTypes;
using DeepReader.Apis.GraphQl.DataLoaders;
using DeepReader.Apis.GraphQl.SubscriptionTypes;
using DeepReader.Apis.JsonSourceGenerators;
using DeepReader.Apis.Options;
using DeepReader.Storage;
using DeepReader.Storage.HealthChecks.Faster;
using HealthChecks.UI.Client;
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
                        options.JsonSerializerOptions.MaxDepth = 10;
                        options.JsonSerializerOptions.IncludeFields = true;
                        options.JsonSerializerOptions.AddContext<BlockJsonContext>();
                    });
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen();
                    services
                        .AddGraphQLServer()
                         .ModifyOptions(options =>
                         {
                             options.DefaultBindingBehavior = BindingBehavior.Explicit;
                         })
                        .AddInMemorySubscriptions()
                        .AddQueryType(q => q.Name("Query"))
                            .AddType<BlockQueryType>()
                            .AddType<TransactionQueryType>()
                        .AddSubscriptionType(s => s.Name("Subscription"))
                            .AddType<BlockSubscriptionType>()
                            .AddType<TransactionSubscriptionType>()
                        .AddDataLoader<BlockByIdDataLoader>()
                        .AddDataLoader<BlocksWithTracesByIdDataLoader>()
                        .AddDataLoader<TransactionByIdDataLoader>();
                    services
                        .AddHealthChecks()
                        .AddCheck<ReadCacheEnabledHealthCheck>("ReadCacheEnabled")
                        .AddCheck<MaxBlocksCacheEntriesHealthCheck>("MaxBlocksCacheEntries")
                        .AddCheck<MaxTransactionsCacheEntriesHealthCheck>("MaxTransactionsCacheEntries")
                        .AddCheck<CheckpointIntervalHealthCheck>("CheckpointInterval")
                        .AddCheck<BlocksIndexedHealthCheck>("BlocksIndexed")
                        .AddCheck<TransactionsIndexedHealthCheck>("TransactionsIndexed");
                    services
                        .AddHealthChecksUI()
                        .AddInMemoryStorage();
                    services.AddSingleton<MetricsCollector>();

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
                        endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                        {
                            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                        });
                        endpoints.MapHealthChecksUI();
                    });
                });
            });
            return builder;
        }
    }
}