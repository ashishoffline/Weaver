using System.Data;
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
    public Task<IReadOnlyList<T>> QueryAsync<T>(string sqlQuery, IEnumerable<KeyValuePair<string, object>> parameters, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
