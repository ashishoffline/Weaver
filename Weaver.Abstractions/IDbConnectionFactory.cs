using System.Data.Common;

namespace Weaver.Abstractions;

public interface IDbConnectionFactory
{
    DbConnection GetConnection();
}
