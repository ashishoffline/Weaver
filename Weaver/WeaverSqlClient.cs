using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Weaver.Abstractions;

namespace Weaver;

/// <summary>
/// A concrete implementation of <see cref="IDbClient"/> that provides database access using a connection factory.
/// </summary>
/// <remarks>
/// <para>
/// This class is responsible for executing SQL queries, mapping results to objects, 
/// and managing connections obtained from an <see cref="IDbConnectionFactory"/>.
/// </para>
/// <para>
/// It supports asynchronous operations, parameterized queries, and proper cancellation via <see cref="CancellationToken"/>.
/// </para>
/// <para>
/// Typically used in repository or service classes to interact with the database in a safe and reusable manner.
/// </para>
/// </remarks>
public sealed class WeaverSqlClient : IDbClient
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeaverSqlClient"/> class.
    /// </summary>
    /// <param name="dbConnectionFactory">
    /// The <see cref="IDbConnectionFactory"/> used to create database connections.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="dbConnectionFactory"/> is <c>null</c>.
    /// </exception>
    public WeaverSqlClient(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
    }

    ///<inheritdoc/>
    public IAsyncEnumerable<T> QueryStreamAsync<T>(CommandDefinition commandDefinition, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        return MapperCache.GetMapper<T>().StreamFromReaderAsync(null!, cancellationToken);
    }

    ///<inheritdoc/>
    public async Task<IReadOnlyList<T>> QueryAsync<T>(CommandDefinition commandDefinition, CancellationToken cancellationToken = default)
    {
        return await MapperCache.GetMapper<T>().MapAllFromReaderAsync(null!, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> QueryAsync<T>(CommandDefinition commandDefinition, Func<DbDataReader, CancellationToken, Task<T>> map, CancellationToken cancellationToken = default)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        return await map(null!, cancellationToken).ConfigureAwait(false);
    }

    public Task<DbDataReader> ExecuteReaderAsync(CommandDefinition commandDefinition, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public async Task<T?> QueryFirstOrDefaultAsync<T>(CommandDefinition commandDefinition, CancellationToken cancellationToken = default)
    {
        return await MapperCache.GetMapper<T>().MapFirstFromReaderAsync(null!, cancellationToken).ConfigureAwait(false);
    }

    ///<inheritdoc/>
    public async Task<T?> ExecuteScalarAsync<T>(CommandDefinition commandDefinition, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public async Task<object?> ExecuteScalarAsync(CommandDefinition commandDefinition, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public async Task<int> ExecuteAsync(CommandDefinition commandDefinition, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public async Task<int> ExecuteAsync<T>(Func<DbConnection, DbTransaction, CancellationToken, Task<int>> func, IsolationLevel? isolationLevel = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public async Task<int> ExecuteAsync(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandType = CommandType.Text, IEnumerable<KeyValuePair<string, object>>? parameters = null, int commandTimeoutInSeconds = 30, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
