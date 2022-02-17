using DeepReader.Apis.GraphQl.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeepReader.Apis.GraphQl
{
    /// <summary>
    /// Extends <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> with Serilog configuration methods.
    /// </summary>
    public static class DeepReaderGrapqhHostBuilderExtensions
    {
        /// <summary>Sets Faster as the logging provider.</summary>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="T:Serilog.Log" /> will be used.</param>
        /// <param name="dispose">When <c>true</c>, dispose <paramref name="logger" /> when the framework disposes the provider. If the
        /// logger is not specified but <paramref name="dispose" /> is <c>true</c>, the <see cref="M:Serilog.Log.CloseAndFlush" /> method will be
        /// called on the static <see cref="T:Serilog.Log" /> class instead.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder UseDeepReaderGraphQl(
            this IHostBuilder builder,
            ILogger logger = null,
            bool dispose = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.ConfigureServices((_, collection) =>
            {
                collection
                    .AddGraphQLServer()
                    .AddQueryType<BlockQuery>()
                    .AddQueryType<TransactionQuery>()
                    .AddQueryType<ActionQuery>();

                // TODO 
                collection.AddInMemorySubscriptions();
            });
            return builder;
        }
    }
}
