using System.Data.Common;

namespace Weaver.Abstractions;

/// <summary>
/// Represents a factory for creating <see cref="DbConnection"/> instances.
/// </summary>
/// <remarks>
/// Implementations of this interface provide a centralized way to create 
/// database connections, which can simplify dependency injection and 
/// connection management in applications.
/// </remarks>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates and returns a new <see cref="DbConnection"/> instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="DbConnection"/> object that can be used to interact with the database.
    /// </returns>
    /// <remarks>
    /// The caller is responsible for opening and disposing the connection 
    /// when finished. Implementations may return a connection with a predefined
    /// connection string or other configuration settings.
    /// </remarks>
    DbConnection GetConnection();
}
