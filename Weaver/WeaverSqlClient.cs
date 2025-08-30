using System.Data;
using Weaver.Abstractions;

namespace Weaver;

public sealed class WeaverSqlClient : IDbClient
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public WeaverSqlClient(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    public Task<IReadOnlyList<T>> QueryAsync<T>(string sqlQuery, IEnumerable<KeyValuePair<string, object>> parameters, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
