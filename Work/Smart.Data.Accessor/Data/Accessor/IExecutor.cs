namespace Smart.Data.Accessor
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    public interface IExecutor
    {
        int Execute(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);

        T ExecuteScalar<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);

        IDataReader ExecuteReader(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);

        IEnumerable<T> Query<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);

        T QueryFirstOrDefault<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);

        // Async

        Task<int> ExecuteAsync(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);

        Task<T> ExecuteScalarAsync<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);

        Task<IDataReader> ExecuteReaderAsync(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);

        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);

        Task<T> QueryFirstOrDefaultAsync<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);
    }
}
