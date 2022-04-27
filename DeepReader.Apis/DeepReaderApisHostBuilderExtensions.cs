using System.Text.Json;
using DeepReader.Apis.GraphQl.QueryTypes;
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
                    services.Configure<ApiOptions>(config => hostContext.Configuration.GetSection("ElasticStorageOptions").Bind(config));
                    services.AddControllers().AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.MaxDepth = Int32.MaxValue;
                        options.JsonSerializerOptions.IncludeFields = true;
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    });
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen();

                    services.AddGraphQLServer()
                        .ModifyOptions(options =>
                        {
                            options.DefaultBindingBehavior = BindingBehavior.Explicit;
                        })
                        .AddQueryType(q => q.Name("Query"))
                        .AddType<BlockQueryType>();
                });
                webBuilder.Configure(app =>
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DeepReaderAPI v1");
                    });
                    //app.UseHttpsRedirection();
                    app.UseRouting();

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
                });
            });
            return builder;
        }
    }
}