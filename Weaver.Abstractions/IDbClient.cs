using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Weaver.Abstractions
{
    public interface IDbClient
    {
        Task<IReadOnlyList<T>> QueryAsync<T>(string sqlQuery, IEnumerable<KeyValuePair<string, object>> parameters, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default);
    }
}
