using System.Data.Common;
using Weaver.Abstractions;

namespace Weaver;

public sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly DbProviderFactory _dbProviderFactory;
    private readonly string _connectionString;

    public DbConnectionFactory(DbProviderFactory dbProviderFactory, string connectionString)
    {
        _dbProviderFactory = dbProviderFactory;
        _connectionString = connectionString;
    }

    public DbConnection GetConnection()
    {
        DbConnection connection = _dbProviderFactory.CreateConnection() ?? throw new NullReferenceException("Couldn't create connection objection.");
        connection.ConnectionString = _connectionString;
        return connection;
    }
}