using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Forge.TransactionHandler.ServiceConfiguration;
public static class TransactionHandlerServiceConfiguration
{
    /// <summary>
    /// Adds the the <see cref="TransactionHandler"/> to the middleware pipeline.
    /// 
    /// The default behaviour of the <see cref="TransactionHandler"/> is to 
    /// persist data after a 200 type status code is returned. This behaviour
    /// can be overriden by passing this method a config builder
    /// </summary>
    /// <param name="app"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IApplicationBuilder UseTransactionHandler(
        this IApplicationBuilder app,
        Func<TransactionHandlerConfig, TransactionHandlerConfig>? builder = null
        )
    {
        if (TransactionHandler.Configuration is not null)
        {
            throw new InvalidOperationException("Only one Transaction Handler can be used");
        }

        var config = new TransactionHandlerConfig();

        builder?.Invoke(config);

        TransactionHandler.Configuration = config;

        return app.UseMiddleware<TransactionHandler>();
    }

    /// <summary>
    /// Adds an <see cref="ITransactionGroup"/> to the DI container.
    /// 
    /// <para>
    /// The tranaction group is added as a keyed service to the DI container
    /// The key used is the naem provided in the config builder.
    /// 
    /// In order to access a specific transaction group in a service the <see cref="ResolveTransactionGroupAttribute"/>
    /// can be used
    /// </para>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddTransactionGroup(
        this IServiceCollection services,
        Func<TransactionGroupConfig.INameConfig, TransactionGroupConfig.IContextConfig> builder)
    {
        var group = (TransactionGroupConfig)builder(new TransactionGroupConfig());

        if (group is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        services.AddKeyedScoped<ITransactionGroup>(
            group.Name,
            (provider, key) => new TransactionGroup(
                group.Name,
                group.Contexts
                    .Select(g => (DbContext)provider.GetRequiredService(g))
                    .ToList()
                )
            );

        return services.AddScoped(
            provider => (ITransactionGroup)provider.GetRequiredKeyedService(typeof(ITransactionGroup),
            group.Name)
            );
    }
}
