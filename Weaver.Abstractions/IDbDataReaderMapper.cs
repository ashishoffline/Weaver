using System.ComponentModel;
using System.Data.Common;

namespace Weaver.Abstractions;

/// <summary>
/// Internal marker interface used for runtime discovery of source-generated data reader mappers.
/// This interface is not intended for direct implementation or consumption by external code.
/// </summary>
/// <remarks>
/// Implemented implicitly by <see cref="IDbDataReaderMapper{T}"/> to enable type-agnostic reflection
/// and caching of mapper instances. Hidden from IntelliSense via <see cref="EditorBrowsableAttribute"/>.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Internal marker interface used for runtime discovery of source-generated data reader mappers.")]
public interface IDbDataReaderMapper { }

/// <summary>
/// Defines a contract for mapping rows from a <see cref="DbDataReader"/> into instances of type <typeparamref name="T"/>.
/// Implemented by source-generated mappers to provide efficient, reflection-free data access.
/// </summary>
/// <typeparam name="T">The target type to be mapped from the data reader.</typeparam>
public interface IDbDataReaderMapper<T> : IDbDataReaderMapper
{
    /// <summary>
    /// Maps all rows from the specified <see cref="DbDataReader"/> into a read-only list of <typeparamref name="T"/> instances.
    /// </summary>
    /// <param name="reader">The data reader containing the result set to map.</param>
    /// <param name="cancellationToken">A cancellation token to observe while reading.</param>
    /// <returns>A task that resolves to a read-only list of mapped <typeparamref name="T"/> instances.</returns>
    Task<IReadOnlyList<T>> MapAllFromReaderAsync(DbDataReader reader, CancellationToken cancellationToken);

    /// <summary>
    /// Maps the first row from the specified <see cref="DbDataReader"/> into a single <typeparamref name="T"/> instance.
    /// Returns <see langword="null"/> if no rows are available.
    /// </summary>
    /// <param name="reader">The data reader containing the result set to map.</param>
    /// <param name="cancellationToken">A cancellation token to observe while reading.</param>
    /// <returns>A task that resolves to the first mapped <typeparamref name="T"/> instance, or <c>null</c> if none.</returns>
    Task<T?> MapFirstFromReaderAsync(DbDataReader reader, CancellationToken cancellationToken);

    /// <summary>
    /// Streams rows from the specified <see cref="DbDataReader"/> as an asynchronous sequence of <typeparamref name="T"/> instances.
    /// </summary>
    /// <param name="reader">The data reader containing the result set to stream.</param>
    /// <param name="cancellationToken">A cancellation token to observe while reading.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields mapped <typeparamref name="T"/> instances.</returns>
    IAsyncEnumerable<T> StreamFromReaderAsync(DbDataReader reader, CancellationToken cancellationToken);
}
