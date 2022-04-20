using System.Text.Json;
using DeepReader.Apis.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using Swashbuckle.AspNetCore;

namespace DeepReader.Apis.REST
{
    /// <summary>
    /// Extends <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> with Serilog configuration methods.
    /// </summary>
    public static class DeepReaderRestHostBuilderExtensions
    {
        /// <summary>Sets Faster as the logging provider.</summary>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="T:Serilog.Log" /> will be used.</param>
        /// <param name="dispose">When <c>true</c>, dispose <paramref name="logger" /> when the framework disposes the provider. If the
        /// logger is not specified but <paramref name="dispose" /> is <c>true</c>, the <see cref="M:Serilog.Log.CloseAndFlush" /> method will be
        /// called on the static <see cref="T:Serilog.Log" /> class instead.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder UseDeepReaderRest(
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
                        endpoints.MapControllers();
                        endpoints.MapMetrics();
                    });
                });
            });
            return builder;
        }
    }
}
