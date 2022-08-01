using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace WebApi.Store.Sql
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly IOptions<SqlDatabaseSettings> _options;

        public SqlConnectionFactory(IOptions<SqlDatabaseSettings> options)
        {
            _options = options;
        }
        
        public IDbConnection CreateConnection()
        {
            var con = new SqliteConnection(_options.Value.ConnectionString);
            
            return new SqliteConnection(_options.Value.ConnectionString);
        }
    }
}