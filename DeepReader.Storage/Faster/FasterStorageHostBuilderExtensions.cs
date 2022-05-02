using DeepReader.Storage.Options;
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
            builder.ConfigureServices((hostContext, services) =>
            {
                services.Configure<FasterStorageOptions>(config => hostContext.Configuration.GetSection("FasterStorageOptions").Bind(config));
                services.AddSingleton(provider =>
                {
                    IStorageAdapter storageAdapter = new FasterStorage(provider.GetRequiredService<IOptionsMonitor<FasterStorageOptions>>());
                    return storageAdapter;
                });
            });
            return builder;
        }
    }
}
