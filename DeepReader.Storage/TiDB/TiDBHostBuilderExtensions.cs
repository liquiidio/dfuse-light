using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DeepReader.Storage.TiDB
{
    /// <summary>
    /// Extends <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> with Serilog configuration methods.
    /// </summary>
    public static class TiDBHostBuilderExtensions
    {
        /// <summary>Sets Faster as the logging provider.</summary>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="T:Serilog.Log" /> will be used.</param>
        /// <param name="dispose">When <c>true</c>, dispose <paramref name="logger" /> when the framework disposes the provider. If the
        /// logger is not specified but <paramref name="dispose" /> is <c>true</c>, the <see cref="M:Serilog.Log.CloseAndFlush" /> method will be
        /// called on the static <see cref="T:Serilog.Log" /> class instead.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder UseTiDB(
            this IHostBuilder builder,
            ILogger logger = null!,
            bool dispose = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.ConfigureServices((hostContext, services) =>
            {
                var connectionString = hostContext.Configuration["ConnectionStrings:TiDBConnection"];
                services.AddPooledDbContextFactory<DataContext>((serviceProvider, options) =>
                {
                    options.UseMySql(connectionString, MySqlServerVersion.AutoDetect(connectionString), x =>
                    {
                        x.MigrationsAssembly("DeepReader");
                    });
                });

                services.AddSingleton<BlockRepository>();
                services.AddSingleton<TransactionRepository>();
                services.AddSingleton<ActionTraceRepository>();

                services.AddSingleton(provider =>
                {
                    IStorageAdapter storageAdapter = new TiDBStorage(
                        provider.GetRequiredService<BlockRepository>(),
                        provider.GetRequiredService<TransactionRepository>(),
                        provider.GetRequiredService<ActionTraceRepository>());
                    return storageAdapter;
                });
            });
            return builder;
        }
    }
}