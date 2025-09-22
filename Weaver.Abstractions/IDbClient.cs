using System.Data;
using System.Data.Common;

namespace Weaver.Abstractions;

/// <summary>
/// Represents a database client capable of executing queries and returning results.
/// </summary>
public interface IDbClient
{
    /// <summary>
    /// Executes the specified database command asynchronously and returns a stream of results of type <typeparamref
    /// name="T"/> as an asynchronous enumerable.
    /// </summary>
    /// <remarks>Enumeration of the returned stream begins executing the command and reading results. The
    /// query is executed lazily as the results are consumed. The caller is responsible for enumerating the results and
    /// handling cancellation if needed.</remarks>
    /// <typeparam name="T">The type of the elements returned in the result stream.</typeparam>
    /// <param name="commandDefinition">The database command to execute, including the SQL statement and any associated parameters.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields each result of type <typeparamref name="T"/> as they are read
    /// from the database.</returns>
    IAsyncEnumerable<T> QueryStreamAsync<T>(CommandDefinition commandDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the specified database command asynchronously and returns a read-only list of result items of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements to be returned in the result set.</typeparam>
    /// <param name="commandDefinition">An object that defines the SQL command to execute, including the command text, parameters, and execution
    /// options.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation. The default value is <see
    /// cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a read-only list of items of type
    /// <typeparamref name="T"/> returned by the query. If no rows are returned, the list will be empty.</returns>
    Task<IReadOnlyList<T>> QueryAsync<T>(CommandDefinition commandDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the specified database command asynchronously and maps the result using the provided mapping function.
    /// </summary>
    /// <remarks>The command is executed asynchronously using the provided <paramref
    /// name="commandDefinition"/>. The <paramref name="map"/> function is invoked to transform the data reader into the
    /// desired result. The operation can be cancelled by passing a cancellation token.</remarks>
    /// <typeparam name="T">The type of the result returned by the mapping function.</typeparam>
    /// <param name="commandDefinition">The database command to execute, including the SQL statement and any associated parameters.</param>
    /// <param name="map">A function that processes the <see cref="DbDataReader"/> and returns a result of type <typeparamref name="T"/>.
    /// The function receives the data reader and a cancellation token.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see langword="default"/>.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the value returned by the mapping
    /// function.</returns>
    Task<T> QueryAsync<T>(CommandDefinition commandDefinition, Func<DbDataReader, CancellationToken, Task<T>> map, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the specified command asynchronously and returns the first result mapped to the specified type, or the
    /// default value if no results are found.
    /// </summary>
    /// <typeparam name="T">The type to which the result is mapped. Must be compatible with the query result set.</typeparam>
    /// <param name="commandDefinition">An object that defines the SQL command to execute, including the command text, parameters, and execution
    /// options.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation. The default value is <see
    /// cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the first mapped result of type
    /// <typeparamref name="T"/>, or the default value for <typeparamref name="T"/> if no rows are returned.</returns>
    Task<T?> QueryFirstOrDefaultAsync<T>(CommandDefinition commandDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously executes the specified command and returns a data reader for reading the results.
    /// </summary>
    /// <remarks>The returned <see cref="DbDataReader"/> must be disposed when no longer needed. The caller is
    /// responsible for managing the lifetime of the reader and handling any exceptions that may occur during
    /// execution.</remarks>
    /// <param name="commandDefinition">The command to execute, including the SQL statement, parameters, and execution options.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation. The result contains a <see cref="DbDataReader"/> for reading the
    /// command results.</returns>
    Task<DbDataReader> ExecuteReaderAsync(CommandDefinition commandDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the specified command asynchronously and returns the first column of the first row in the result set,
    /// cast to the specified type.
    /// </summary>
    /// <remarks>If the query returns multiple rows or columns, only the first column of the first row is
    /// returned. This method is typically used for queries that return a single value, such as aggregate
    /// functions.</remarks>
    /// <typeparam name="T">The type to which the result will be cast. Must be compatible with the value returned by the query.</typeparam>
    /// <param name="commandDefinition">An object that defines the SQL command to execute, including the command text, parameters, and execution
    /// options.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the value of the first column of the
    /// first row in the result set, cast to <typeparamref name="T"/>. Returns <see langword="null"/> if the result set
    /// is empty or the value is <see langword="DBNull"/>.</returns>
    Task<T?> ExecuteScalarAsync<T>(CommandDefinition commandDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the specified command asynchronously and returns the first column of the first row in the result set.
    /// </summary>
    /// <param name="commandDefinition">The command to execute, including the SQL statement and any associated parameters.</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation. The result contains the value of the first column of the first
    /// row in the result set, or <see langword="null"/> if the result set is empty.</returns>
    Task<object?> ExecuteScalarAsync(CommandDefinition commandDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the specified database command asynchronously and returns the number of rows affected.
    /// </summary>
    /// <param name="commandDefinition">The command to execute, including the SQL statement and any associated parameters. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation is canceled if the token is triggered before
    /// completion.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the
    /// command.</returns>
    Task<int> ExecuteAsync(CommandDefinition commandDefinition, CancellationToken cancellationToken = default);

    Task<int> ExecuteAsync<T>(Func<DbConnection, DbTransaction, CancellationToken, Task<int>> func, IsolationLevel? isolationLevel = null, CancellationToken cancellationToken = default);

    Task<int> ExecuteAsync(DbConnection connection, DbTransaction transaction, string commandText, CommandType commandType = CommandType.Text, IEnumerable<KeyValuePair<string, object>>? parameters = null, int commandTimeoutInSeconds = 30, CancellationToken cancellationToken = default);
}
