using System;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DataLibrary.Engine
{
    using System.Data;
    using System.Threading.Tasks;

    public class ExecuteEngine
    {
        //private const CommandBehavior CommandBehaviorQueryWithClose =
        //    CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess;

        //private const CommandBehavior CommandBehaviorQuery =
        //    CommandBehavior.SequentialAccess;

        //private const CommandBehavior CommandBehaviorQueryFirstOrDefaultWithClose =
        //    CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleRow;

        //private const CommandBehavior CommandBehaviorQueryFirstOrDefault =
        //    CommandBehavior.SequentialAccess | CommandBehavior.SingleRow;

        // TODO Config

        //--------------------------------------------------------------------------------
        // Core
        //--------------------------------------------------------------------------------

        private static IDbCommand SetupCommand(IDbConnection con, IDbTransaction transaction, string sql, CommandType commandType, int? commandTimeout)
        {
            var cmd = con.CreateCommand();

            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            cmd.CommandType = commandType;
            cmd.CommandText = sql;

            if (commandTimeout.HasValue)
            {
                cmd.CommandTimeout = commandTimeout.Value;
            }

            return cmd;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DbCommand SetupAsyncCommand(IDbConnection con, IDbTransaction transaction, string sql, CommandType commandType, int? commandTimeout)
        {
            if (SetupCommand(con, transaction, sql, commandType, commandTimeout) is DbCommand dbCommand)
            {
                return dbCommand;
            }

            throw new EngineException("Async operation is not supported.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2007:DoNotDirectlyAwaitATask", Justification = "Ignore")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task OpenAsync(IDbConnection con, CancellationToken cancel)
        {
            if (con is DbConnection dbConnection)
            {
                await dbConnection.OpenAsync(cancel);
            }
            else
            {
                throw new EngineException("Async operation is not supported.");
            }
        }

        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        // TODO パラメータは1インスタンスのIEvalにして、複数の時はAggregateEval(Array, List)を指定する形にするか？

        public int Execute(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandType, commandTimeout))
            {
                // TODO parameter
                //var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                //builder.Build?.Invoke(cmd, param);

                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    var result = cmd.ExecuteNonQuery();

                    // TODO parameter
                    //builder.PostProcess?.Invoke(cmd, param);

                    return result;
                }
                finally
                {
                    if (wasClosed)
                    {
                        con.Close();
                    }
                }
            }
        }

        public async Task<int> ExecuteAsync(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout,
            CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupAsyncCommand(con, transaction, sql, commandType, commandTimeout))
            {
                // TODO parameter
                //var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                //builder.Build?.Invoke(cmd, param);

                try
                {
                    if (wasClosed)
                    {
                        await OpenAsync(con, cancel).ConfigureAwait(false);
                    }

                    var result = await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);

                    // TODO parameter
                    //builder.PostProcess?.Invoke(cmd, param);

                    return result;
                }
                finally
                {
                    if (wasClosed)
                    {
                        con.Close();
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        public static T ExecuteScalar<T>(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandType, commandTimeout))
            {
                // TODO
                //var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                //builder.Build?.Invoke(cmd, param);

                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    var result = cmd.ExecuteScalar();

                    // TODO
                    // builder.PostProcess?.Invoke(cmd, param);

                    if (result is DBNull)
                    {
                        return default;
                    }

                    if (result is T scalar)
                    {
                        return scalar;
                    }

                    // TODO config
                    //var parser = config.CreateParser(result.GetType(), typeof(T));
                    //return (T)parser(result);
                    return default;
                }
                finally
                {
                    if (wasClosed)
                    {
                        con.Close();
                    }
                }
            }
        }

        public static async Task<T> ExecuteScalarAsync<T>(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout,
            CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupAsyncCommand(con, transaction, sql, commandType, commandTimeout))
            {
                // TODO
                //var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                //builder.Build?.Invoke(cmd, param);

                try
                {
                    if (wasClosed)
                    {
                        await OpenAsync(con, cancel).ConfigureAwait(false);
                    }

                    var result = await cmd.ExecuteScalarAsync(cancel).ConfigureAwait(false);

                    // TODO
                    //builder.PostProcess?.Invoke(cmd, param);

                    if (result is DBNull)
                    {
                        return default;
                    }

                    if (result is T scalar)
                    {
                        return scalar;
                    }

                    // TODO config
                    //var parser = config.CreateParser(result.GetType(), typeof(T));
                    //return (T)parser(result);
                    return default;
                }
                finally
                {
                    if (wasClosed)
                    {
                        con.Close();
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------
        // ExecuteReader
        //--------------------------------------------------------------------------------

        public static IDataReader ExecuteReader(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout,
            CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var cmd = default(IDbCommand);
            var reader = default(IDataReader);
            try
            {
                cmd = SetupCommand(con, transaction, sql, commandType, commandTimeout);
                // TODO
                //var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                //builder.Build?.Invoke(cmd, param);

                if (wasClosed)
                {
                    con.Open();
                }

                reader = cmd.ExecuteReader(wasClosed
                    ? commandBehavior | CommandBehavior.CloseConnection
                    : commandBehavior);
                wasClosed = false;

                // TODO
                //builder.PostProcess?.Invoke(cmd, param);

                // TODO reader
                //return new WrappedReader(cmd, reader);
                return null;
            }
            catch (Exception)
            {
                reader?.Dispose();
                cmd?.Dispose();
                throw;
            }
            finally
            {
                if (wasClosed)
                {
                    con.Close();
                }
            }
        }

        // TODO

        //--------------------------------------------------------------------------------
        // Query
        //--------------------------------------------------------------------------------

        // TODO

        //--------------------------------------------------------------------------------
        // QueryFirstOrDefault
        //--------------------------------------------------------------------------------

        // TODO
    }
}
