using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Weaver.Abstractions;

namespace Weaver.Extensions;

/// <summary>
/// Provides extension methods for registering Weaver database services 
/// into an <see cref="IServiceCollection"/>.
/// </summary>
/// <remarks>
/// These extensions simplify dependency injection (DI) setup by adding the 
/// required database components (e.g., <see cref="IDbConnectionFactory"/> 
/// and <see cref="IDbClient"/>) into the service container.
/// </remarks>
public static class WeaverExtensions
{
    /// <summary>
    /// Registers an <see cref="IDbConnectionFactory"/> implementation in the 
    /// dependency injection container using the specified <see cref="DbProviderFactory"/> 
    /// and connection string.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the service registration to.
    /// </param>
    /// <param name="dbProviderFactory">
    /// The <see cref="DbProviderFactory"/> used to create database connections.
    /// </param>
    /// <param name="connectionString">
    /// The database connection string to be used by the connection factory.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.
    /// </returns>
    /// <remarks>
    /// This method registers <see cref="IDbConnectionFactory"/> as a singleton, ensuring that 
    /// a single factory instance is reused throughout the application's lifetime.
    /// </remarks>
    public static IServiceCollection AddConnectionFactory(this IServiceCollection services, DbProviderFactory dbProviderFactory, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(dbProviderFactory, connectionString));
        return services;
    }
    /// <summary>
    /// Registers the <see cref="WeaverSqlClient"/> as the implementation for <see cref="IDbClient"/> 
    /// in the dependency injection container.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the service registration to.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method registers <see cref="IDbClient"/> as a singleton. It resolves an 
    /// <see cref="IDbConnectionFactory"/> from the service provider and uses it to create 
    /// a <see cref="WeaverSqlClient"/> instance.
    /// </para>
    /// <para>
    /// Make sure <see cref="IDbConnectionFactory"/> has been registered in the container 
    /// (for example, by calling <c>AddConnectionFactory</c>) before using this method.
    /// </para>
    /// </remarks>
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