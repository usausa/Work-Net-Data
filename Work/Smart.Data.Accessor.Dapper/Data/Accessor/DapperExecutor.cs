namespace Smart.Data.Accessor
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    using Dapper;

    public sealed class DapperExecutor : IExecutor
    {
        public int Execute(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.Execute(sql, (parameter as DapperParameter)?.Parameters, tx, timeout, commandType);
        }

        public T ExecuteScalar<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.ExecuteScalar<T>(sql, (parameter as DapperParameter)?.Parameters, tx, timeout, commandType);
        }

        public IDataReader ExecuteReader(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.ExecuteReader(sql, (parameter as DapperParameter)?.Parameters, tx, timeout, commandType);
        }

        public IEnumerable<T> Query<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.Query<T>(sql, (parameter as DapperParameter)?.Parameters, tx, false, timeout, commandType);
        }

        public T QueryFirstOrDefault<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.QueryFirstOrDefault<T>(sql, (parameter as DapperParameter)?.Parameters, tx, timeout, commandType);
        }

        // Async

        public Task<int> ExecuteAsync(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.ExecuteAsync(sql, (parameter as DapperParameter)?.Parameters, tx, timeout, commandType);
        }

        public Task<T> ExecuteScalarAsync<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.ExecuteScalarAsync<T>(sql, (parameter as DapperParameter)?.Parameters, tx, timeout, commandType);
        }

        public Task<IDataReader> ExecuteReaderAsync(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.ExecuteReaderAsync(sql, (parameter as DapperParameter)?.Parameters, tx, timeout, commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.QueryAsync<T>(sql, (parameter as DapperParameter)?.Parameters, tx, timeout, commandType);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.QueryFirstOrDefaultAsync<T>(sql, (parameter as DapperParameter)?.Parameters, tx, timeout, commandType);
        }
    }
}
