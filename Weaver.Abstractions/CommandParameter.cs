using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Weaver.Abstractions;

/// <summary>
/// Represents a parameter used in a database command, including its name, value, database type, direction, and optional type name.
/// </summary>
/// <remarks>
/// This struct encapsulates metadata for parameters used in database commands, including stored procedures and parameterized queries.
/// It supports specifying the parameter's name, value, ADO.NET type, direction, and an optional type name for custom database types.
/// Instances are immutable and can be safely passed across components.
/// </remarks>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
[DebuggerDisplay("{Name} = {Value}")]
public readonly struct CommandParameter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandParameter"/> struct with the specified name and value.
    /// </summary>
    /// <param name="name">The name of the parameter as expected by the database command. Cannot be <see langword="null"/>.</param>
    /// <param name="value">The value to assign to the parameter. If <see langword="null"/>, it will be treated as <see cref="DBNull.Value"/> during execution.</param>
    /// <remarks>
    /// This constructor sets the <see cref="ParameterDirection"/> to <see cref="ParameterDirection.Input"/> by default.
    /// Use this overload for simple input parameters in text-based or stored procedure commands.
    /// </remarks>
    public CommandParameter(string name, object? value)
    {
        Name = name;
        Value = value;
        ParameterDirection = ParameterDirection.Input;
    }

    /// <summary>
    /// Gets the name of the parameter as expected by the database command.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the value associated with this instance.
    /// Gets the value assigned to the parameter. If <see langword="null"/>, it will be treated as <see cref="DBNull.Value"/> during execution.
    /// </summary>
    public object? Value { get; init; }

    /// <summary>
    /// Gets the ADO.NET database type of the parameter, if explicitly specified.
    /// </summary>
    public DbType? DbType { get; init; }

    /// <summary>
    /// Gets the direction of the parameter, indicating whether it is input, output, bidirectional, or a return value.
    /// </summary>
    public ParameterDirection ParameterDirection { get; init; }

    /// <summary>
    /// Gets the optional database type name used for custom or user-defined types.
    /// </summary>
    public string? TypeName { get; init; }

    /// <summary>
    /// Creates a new <see cref="CommandParameter"/> with the specified name and value.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value to assign to the parameter.</param>
    /// <returns>A new <see cref="CommandParameter"/> instance with default input direction.</returns>
    public static CommandParameter Create(string name, object? value)
    {
        return new CommandParameter(name, value);
    }
}
