using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Weaver.Abstractions;

namespace Weaver.Extensions;

public static class WeaverExtensions
{
    public static IServiceCollection AddConnectionFactory(this IServiceCollection services, DbProviderFactory dbProviderFactory, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(sp =>
        {
            return new DbConnectionFactory(dbProviderFactory, connectionString);
        });

        return services;
    }
    public static IServiceCollection AddWeaverSqlClient(this IServiceCollection services)
    {
        services.AddSingleton<IDbClient>(sp =>
        {
            var dbConnectionFactory = sp.GetRequiredService<IDbConnectionFactory>();
            return new WeaverSqlClient(dbConnectionFactory);
        });

        return services;
    }
}
