using System.Data.Common;
using Weaver.Abstractions;

namespace Weaver;

    /// <summary>
    /// A factory for creating <see cref="DbConnection"/> instances using a specified <see cref="DbProviderFactory"/>.
    /// </summary>
    /// <remarks>
    /// This class provides a centralized way to create database connections with a predefined 
    /// connection string and database provider. It is typically used with dependency injection 
    /// to ensure consistent connection creation across the application.
    /// </remarks>
    public sealed class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnectionFactory"/> class.
        /// </summary>
        /// <param name="dbProviderFactory">The database provider factory used to create connections.</param>
        /// <param name="connectionString">The connection string to assign to each connection.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dbProviderFactory"/> or <paramref name="connectionString"/> is null.
        /// </exception>
        public DbConnectionFactory(DbProviderFactory dbProviderFactory, string connectionString)
        {
            _dbProviderFactory = dbProviderFactory ?? throw new ArgumentNullException(nameof(dbProviderFactory));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Creates and returns a new <see cref="DbConnection"/> instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="DbConnection"/> with the configured connection string.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="DbProviderFactory"/> cannot create a connection.</exception>
        public DbConnection GetConnection()
        {
            // Create connection
            DbConnection connection = _dbProviderFactory.CreateConnection() 
                                      ?? throw new InvalidOperationException("DbProviderFactory failed to create a connection.");

            connection.ConnectionString = _connectionString;

            return connection;
        }
    }
