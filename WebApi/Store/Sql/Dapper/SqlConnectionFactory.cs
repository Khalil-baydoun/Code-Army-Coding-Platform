using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;
using webapi.Store.Settings;

namespace WebApi.Store.Sql
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly IOptions<DatabaseConnectionSettings> _options;

        public SqlConnectionFactory(IOptions<DatabaseConnectionSettings> options)
        {
            _options = options;
        }
        
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_options.Value.ConnectionString);
        }
    }
}