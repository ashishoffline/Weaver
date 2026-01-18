using System.Collections.Immutable;
using System.Data;

namespace Weaver.Abstractions;

/// <summary>
/// Represents a fully defined database command, including its text, type, parameters, timeout, and optional transaction isolation level.
/// </summary>
/// <remarks>
/// This immutable struct encapsulates all metadata required to execute a database command consistently across components.
/// It supports both text-based SQL and stored procedures, with optional transaction control and timeout configuration.
/// Once constructed, all properties are read-only, ensuring thread safety and predictability.
/// </remarks>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "There will never be an usecase to compare two CommandDefinition")]
public readonly struct CommandDefinition
{
    /// <summary>
    /// Initializes a new instance of the CommandDefinition class with default values.
    /// </summary>
    /// <remarks>
    /// The default constructor sets <see cref="CommandType"/> to <see cref="CommandType.Text"/>, initializes <see cref="Parameters"/> as an empty
    /// collection, and sets <see cref="CommandTimeoutInSeconds"/> to 30 seconds. These defaults provide a ready-to-use command
    /// definition for standard text-based commands with no parameters and a 30-second timeout.
    /// </remarks>
    public CommandDefinition()
    {
        CommandType = CommandType.Text;
        Parameters = ImmutableArray<CommandParameter>.Empty;
        CommandTimeoutInSeconds = 30;
    }

    /// <summary>
    /// Gets or sets the SQL command text or stored procedure name to execute.
    /// </summary>
    /// <remarks>
    /// This value must be valid for the target database provider. Avoid embedding sensitive data directly in the command text.
    /// </remarks>
    public required string CommandText { get; init; }

    /// <summary>
    /// Gets the type of command to execute, such as text, stored procedure, or table direct.
    /// </summary>
    /// <remarks>
    /// Determines how <see cref="CommandText"/> is interpreted by the database provider. Defaults to <see cref="CommandType.Text"/>.
    /// </remarks>
    public CommandType CommandType { get; init; } = CommandType.Text;

    /// <summary>
    /// Gets the immutable collection of parameters associated with the command.
    /// </summary>
    /// <remarks>
    /// Parameters are defined at construction and cannot be modified. Use <see cref="CommandParameter"/> to specify name, value, and direction.
    /// </remarks>
    public IEnumerable<CommandParameter> Parameters { get; init; } = ImmutableArray<CommandParameter>.Empty;

    /// <summary>
    /// Gets the optional transaction isolation level for the command.
    /// </summary>
    /// <remarks>
    /// If not specified, then explicit transaction is not used during command execution, it will use implicit transaction of the Database.
    /// </remarks>
    public IsolationLevel? IsolationLevel { get; init; }

    /// <summary>
    /// Gets the command timeout duration, in seconds, for database operations.
    /// </summary>
    /// <remarks>
    /// A value of 0 indicates that the command will wait indefinitely for completion. Setting a
    /// higher value may be necessary for long-running queries. The timeout applies to each individual command
    /// execution.
    /// </remarks>

    /// <summary>
    /// Gets the timeout duration, in seconds, for command execution.
    /// </summary>
    /// <remarks>
    /// A value of 0 indicates no timeout. Applies to each execution of the command.
    /// Setting a higher value may be necessary for long-running queries.
    /// </remarks>
    public int CommandTimeoutInSeconds { get; init; } = 30;
}