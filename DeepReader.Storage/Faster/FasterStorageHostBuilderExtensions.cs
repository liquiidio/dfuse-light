using DeepReader.Storage.Faster.Options;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace DeepReader.Storage.Faster
{
    /// <summary>
    /// Extends <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> with Serilog configuration methods.
    /// </summary>
    public static class FasterStorageHostBuilderExtensions
    {
        /// <summary>Sets Faster as the logging provider.</summary>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="T:Serilog.Log" /> will be used.</param>
        /// <param name="dispose">When <c>true</c>, dispose <paramref name="logger" /> when the framework disposes the provider. If the
        /// logger is not specified but <paramref name="dispose" /> is <c>true</c>, the <see cref="M:Serilog.Log.CloseAndFlush" /> method will be
        /// called on the static <see cref="T:Serilog.Log" /> class instead.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder UseFasterStorage(
            this IHostBuilder builder,
            ILogger logger = null,
            bool dispose = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddJsonFile("faster.standalone.settings.json", true, true);
                configBuilder.AddJsonFile("faster.server.settings.json", true, true);
                configBuilder.AddJsonFile("faster.client.settings.json", true, true);
            });

            builder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton(provider =>
                {
                    IStorageAdapter storageAdapter = null;
                    if (hostContext.Configuration.GetSection("FasterStandaloneOptions").GetChildren().Count() != 0)
                    {
                        services.Configure<FasterStandaloneOptions>(config => hostContext.Configuration.GetSection("FasterStandaloneOptions").Bind(config));

                        storageAdapter = new FasterStorage(
                            provider.GetRequiredService<IOptionsMonitor<FasterStandaloneOptions>>(),
                            provider.GetRequiredService<ITopicEventSender>(),
                            provider.GetRequiredService<MetricsCollector>());
                    }
                    else if (hostContext.Configuration.GetSection("FasterServerOptions").GetChildren().Count() != 0)
                    {
                        services.Configure<FasterServerOptions>(config =>
                            hostContext.Configuration.GetSection("FasterServerOptions").Bind(config));

                        storageAdapter = new FasterStorage(
                            provider.GetRequiredService<IOptionsMonitor<FasterServerOptions>>(),
                            provider.GetRequiredService<ITopicEventSender>(),
                            provider.GetRequiredService<MetricsCollector>());
                    }
                    else if (hostContext.Configuration.GetSection("FasterClientOptions").GetChildren().Count() != 0)
                    {
                        services.Configure<FasterClientOptions>(config => hostContext.Configuration.GetSection("FasterClientOptions").Bind(config));

                        storageAdapter = new FasterStorage(
                            provider.GetRequiredService<IOptionsMonitor<FasterClientOptions>>(),
                            provider.GetRequiredService<ITopicEventSender>(),
                            provider.GetRequiredService<MetricsCollector>());
                    }
                    else
                        throw new Exception("Configuration-File for Faster missing");

                    return storageAdapter;
                });
            });
            return builder;
        }
    }
}
