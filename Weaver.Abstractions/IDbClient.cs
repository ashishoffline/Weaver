using System.Data;

namespace Weaver.Abstractions;

/// <summary>
/// Represents a database client capable of executing queries and returning results.
/// </summary>
public interface IDbClient
{
    /// <summary>
    /// Executes the specified SQL query asynchronously and maps the results to a read-only list of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to map each row to.</typeparam>
    /// <param name="sqlQuery">The SQL query string to execute.</param>
    /// <param name="parameters">
    /// The query parameters as key-value pairs. Each key should match a parameter name in the query,
    /// and each value is the parameter value.
    /// </param>
    /// <param name="commandType">
    /// The type of command to execute. Defaults to <see cref="CommandType.Text"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains a read-only list of objects of type <typeparamref name="T"/>.
    /// </returns>
    /// <remarks>
    /// Implementations should handle mapping of database rows to <typeparamref name="T"/>, 
    /// parameter substitution, and connection management. 
    /// Use the cancellation token to support cooperative cancellation.
    /// </remarks>
    Task<IReadOnlyList<T>> QueryAsync<T>(
        string sqlQuery,
        IEnumerable<KeyValuePair<string, object>> parameters,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default);
}
