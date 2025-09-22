using Weaver.Abstractions;

namespace System.Data.Common;

/// <summary>
/// Provides extension methods for <see cref="DbCommand"/> to simplify parameter creation and assignment.
/// </summary>
/// <remarks>
/// These methods streamline the process of adding multiple parameters to a command and creating individual
/// <see cref="DbParameter"/> instances from <see cref="CommandParameter"/> metadata.
/// Intended for use in ADO.NET scenarios where dynamic or bulk parameterization is required.
/// </remarks>
public static class DbCommandExtensions
{
    /// <summary>
    /// Adds a collection of parameters to the specified <see cref="DbCommand"/>.
    /// </summary>
    /// <remarks>
    /// Each <see cref="CommandParameter"/> is converted into a <see cref="DbParameter"/> and added to the <see cref="DbCommand.Parameters"/> collection.
    /// </remarks>
    /// <param name="command">The <see cref="DbCommand"/> to which parameters will be added. Must not be <see langword="null"/>.</param>
    /// <param name="parameters">The collection of parameters to add. Must not be <see langword="null"/>.</param>
    public static void AddParameters(this DbCommand command, IEnumerable<CommandParameter> parameters)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        foreach (var parameter in parameters)
        {
            var param = command.CreateParameter(parameter);
            command.Parameters.Add(param);
        }
    }

    /// <summary>
    /// Creates a new <see cref="DbParameter"/> using the specified <see cref="CommandParameter"/> metadata.
    /// </summary>
    /// <param name="command">The command used to create the parameter. Must not be <see langword="null"/>.</param>
    /// <param name="commandParameter">The metadata describing the parameter's name, value, and direction.</param>
    /// <returns>A configured <see cref="DbParameter"/> instance.</returns>
    private static DbParameter CreateParameter(this DbCommand command, CommandParameter commandParameter)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = commandParameter.Name;
        parameter.Value = commandParameter.Value ?? DBNull.Value;
        parameter.Direction = commandParameter.ParameterDirection;

        if (commandParameter.DbType.HasValue)
        {
            parameter.DbType = commandParameter.DbType.Value;
        }

        return parameter;
    }
}
