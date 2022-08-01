using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApi.Store.Sql.Dapper
{
    public class DapperQueryBuilder
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public DapperQueryBuilder SelectColumns(string tableName, params string[] columns)
        {
            _stringBuilder.Append($"SELECT {CommaSeparated(columns)} FROM {tableName}");
            return this;
        }

        private static string CommaSeparated<T>(IEnumerable<T> columns)
        {
            return string.Join(',', columns);
        }

        public DapperQueryBuilder Where(string condition)
        {
            _stringBuilder.Append($" WHERE {condition}");
            return this;
        }

        public DapperQueryBuilder Where()
        {
            _stringBuilder.Append($" WHERE ");
            return this;
        }

        public DapperQueryBuilder OrderBy(string columnName, string direction)
        {
            _stringBuilder.Append($" ORDER BY {columnName} {direction}");
            return this;
        }

        public DapperQueryBuilder Offset(int offset)
        {
            _stringBuilder.Append($" OFFSET {offset} ROWS");
            return this;
        }

        public DapperQueryBuilder FetchNext(int limit)
        {
            _stringBuilder.Append($" FETCH NEXT {limit} ROWS ONLY");
            return this;
        }

        public DapperQueryBuilder Limit(long offset, int limit)
        {
            _stringBuilder.Append($" LIMIT {offset},{limit}");
            return this;
        }

        public DapperQueryBuilder Limit(int limit)
        {
            _stringBuilder.Append($" LIMIT {limit}");
            return this;
        }

        public DapperQueryBuilder DeleteFrom(string tableName)
        {
            _stringBuilder.Append($"DELETE FROM {tableName}");
            return this;
        }

        public DapperQueryBuilder InsertInto(string tableName, params string[] columns)
        {
            _stringBuilder.Append(
                $@"INSERT INTO {tableName} ({CommaSeparated(columns)}) VALUES ({CommaSeparated(columns.Select(c => $"@{c}"))}); 
                    SELECT LAST_INSERT_ROWID()");
            return this;
        }

        public DapperQueryBuilder Update(string tableName, params string[] columns)
        {
            _stringBuilder.Append($"UPDATE {tableName} SET {CommaSeparated(columns.Select(column => $"{column} = @{column}"))}");
            return this;
        }

        public DapperQueryBuilder Sum(string tableName, string columnName)
        {
            _stringBuilder.Append($"SELECT SUM({columnName}) FROM {tableName}");
            return this;
        }

        public DapperQueryBuilder And(string condition = "")
        {
            _stringBuilder.Append($" AND {condition}");
            return this;
        }

        public DapperQueryBuilder ColumnValueIn<T>(string columnName, IEnumerable<T> values)
        {
            _stringBuilder.Append(InStatement(columnName, values));
            return this;
        }

        public DapperQueryBuilder Count(string tableName)
        {
            _stringBuilder.Append($"SELECT COUNT(*) FROM {tableName}");
            return this;
        }

        public static string InStatement<T>(string columnName, IEnumerable<T> values)
        {
            return $"{columnName} IN({CommaSeparated(values)})";
        }

        public string Build()
        {
            return _stringBuilder.ToString();
        }
    }
}