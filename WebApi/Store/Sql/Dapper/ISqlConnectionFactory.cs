using System.Data;

namespace WebApi.Store.Sql
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}