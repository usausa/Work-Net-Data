namespace DataLibrary.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ExecuteEngine
    {
        private const CommandBehavior CommandBehaviorQueryWithClose =
            CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess;

        private const CommandBehavior CommandBehaviorQuery =
            CommandBehavior.SequentialAccess;

        private const CommandBehavior CommandBehaviorQueryFirstOrDefaultWithClose =
            CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleRow;

        private const CommandBehavior CommandBehaviorQueryFirstOrDefault =
            CommandBehavior.SequentialAccess | CommandBehavior.SingleRow;

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

        public static int Execute(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout,
            Action<IDbCommand> builder,
            Action<IDbCommand> postProcessor)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandType, commandTimeout))
            {
                builder?.Invoke(cmd);

                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    var result = cmd.ExecuteNonQuery();

                    postProcessor?.Invoke(cmd);

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

        public static async Task<int> ExecuteAsync(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout,
            Action<IDbCommand> builder,
            Action<IDbCommand> postProcessor,
            CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupAsyncCommand(con, transaction, sql, commandType, commandTimeout))
            {
                builder?.Invoke(cmd);

                try
                {
                    if (wasClosed)
                    {
                        await OpenAsync(con, cancel).ConfigureAwait(false);
                    }

                    var result = await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);

                    postProcessor?.Invoke(cmd);

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
            int? commandTimeout,
            Action<IDbCommand> builder,
            Action<IDbCommand> postProcessor,
            Func<object, T> converter)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandType, commandTimeout))
            {
                builder?.Invoke(cmd);

                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    var result = cmd.ExecuteScalar();

                    postProcessor?.Invoke(cmd);

                    if (result is DBNull)
                    {
                        return default;
                    }

                    if (result is T scalar)
                    {
                        return scalar;
                    }

                    return converter(result);
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
            Action<IDbCommand> builder,
            Action<IDbCommand> postProcessor,
            Func<object, T> converter,
            CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupAsyncCommand(con, transaction, sql, commandType, commandTimeout))
            {
                builder?.Invoke(cmd);

                try
                {
                    if (wasClosed)
                    {
                        await OpenAsync(con, cancel).ConfigureAwait(false);
                    }

                    var result = await cmd.ExecuteScalarAsync(cancel).ConfigureAwait(false);

                    postProcessor?.Invoke(cmd);

                    if (result is DBNull)
                    {
                        return default;
                    }

                    if (result is T scalar)
                    {
                        return scalar;
                    }

                    return converter(result);
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
            CommandBehavior commandBehavior,
            Action<IDbCommand> builder,
            Action<IDbCommand> postProcessor)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var cmd = default(IDbCommand);
            var reader = default(IDataReader);
            try
            {
                cmd = SetupCommand(con, transaction, sql, commandType, commandTimeout);
                builder?.Invoke(cmd);

                if (wasClosed)
                {
                    con.Open();
                }

                reader = cmd.ExecuteReader(wasClosed
                    ? commandBehavior | CommandBehavior.CloseConnection
                    : commandBehavior);
                wasClosed = false;

                postProcessor?.Invoke(cmd);

                return new WrappedReader(cmd, reader);
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

        public static async Task<IDataReader> ExecuteReaderAsync(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout,
            CommandBehavior commandBehavior,
            Action<IDbCommand> builder,
            Action<IDbCommand> postProcessor,
            CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var cmd = default(DbCommand);
            var reader = default(IDataReader);
            try
            {
                cmd = SetupAsyncCommand(con, transaction, sql, commandType, commandTimeout);
                builder?.Invoke(cmd);

                if (wasClosed)
                {
                    await OpenAsync(con, cancel).ConfigureAwait(false);
                }

                reader = await cmd.ExecuteReaderAsync(wasClosed ? commandBehavior | CommandBehavior.CloseConnection : commandBehavior, cancel).ConfigureAwait(false);
                wasClosed = false;

                postProcessor?.Invoke(cmd);

                return new WrappedReader(cmd, reader);
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

        //--------------------------------------------------------------------------------
        // Query
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<T> ReaderToList<T>(IDataReader reader, Func<IDataRecord, T> mapper)
        {
            var list = new List<T>();
            while (reader.Read())
            {
                list.Add(mapper(reader));
            }

            return list;
        }

        private static IEnumerable<T> ReaderToDefer<T>(IDbCommand cmd, IDataReader reader, Func<IDataRecord, T> mapper)
        {
            using (cmd)
            using (reader)
            {
                while (reader.Read())
                {
                    yield return mapper(reader);
                }
            }
        }

        public static IEnumerable<T> Query<T>(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout,
            bool buffered,
            Action<IDbCommand> builder,
            Action<IDbCommand> postProcessor,
            Func<IDataRecord, T> mapper)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var cmd = default(IDbCommand);
            var reader = default(IDataReader);
            try
            {
                cmd = SetupCommand(con, transaction, sql, commandType, commandTimeout);
                builder?.Invoke(cmd);

                if (wasClosed)
                {
                    con.Open();
                }

                reader = cmd.ExecuteReader(wasClosed ? CommandBehaviorQueryWithClose : CommandBehaviorQuery);
                wasClosed = false;

                postProcessor?.Invoke(cmd);

                if (buffered)
                {
                    return ReaderToList(reader, mapper);
                }
                else
                {
                    var deferred = ReaderToDefer(cmd, reader, mapper);
                    cmd = null;
                    reader = null;
                    return deferred;
                }
            }
            finally
            {
                reader?.Dispose();
                cmd?.Dispose();

                if (wasClosed)
                {
                    con.Close();
                }
            }
        }

        public static async Task<IEnumerable<T>> QueryAsync<T>(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout,
            bool buffered,
            Action<IDbCommand> builder,
            Action<IDbCommand> postProcessor,
            Func<IDataRecord, T> mapper,
            CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var cmd = default(DbCommand);
            var reader = default(DbDataReader);
            try
            {
                cmd = SetupAsyncCommand(con, transaction, sql, commandType, commandTimeout);
                builder?.Invoke(cmd);

                if (wasClosed)
                {
                    await OpenAsync(con, cancel).ConfigureAwait(false);
                }

                reader = await cmd.ExecuteReaderAsync(wasClosed ? CommandBehaviorQueryWithClose : CommandBehaviorQuery, cancel).ConfigureAwait(false);
                wasClosed = false;

                postProcessor?.Invoke(cmd);

                if (buffered)
                {
                    return ReaderToList(reader, mapper);
                }
                else
                {
                    var deferred = ReaderToDefer(cmd, reader, mapper);
                    cmd = null;
                    reader = null;
                    return deferred;
                }
            }
            finally
            {
                reader?.Dispose();
                cmd?.Dispose();

                if (wasClosed)
                {
                    con.Close();
                }
            }
        }

        //--------------------------------------------------------------------------------
        // QueryFirstOrDefault
        //--------------------------------------------------------------------------------

        public static T QueryFirstOrDefault<T>(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout,
            Action<IDbCommand> builder,
            Action<IDbCommand> postProcessor,
            Func<IDataRecord, T> mapper)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandType, commandTimeout))
            {
                builder?.Invoke(cmd);

                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    using (var reader = cmd.ExecuteReader(wasClosed ? CommandBehaviorQueryFirstOrDefaultWithClose : CommandBehaviorQueryFirstOrDefault))
                    {
                        wasClosed = false;

                        postProcessor?.Invoke(cmd);

                        return reader.Read() ? mapper(reader) : default;
                    }
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

        public static async Task<T> QueryFirstOrDefaultAsync<T>(
            IDbConnection con,
            IDbTransaction transaction,
            string sql,
            CommandType commandType,
            int? commandTimeout,
            Action<IDbCommand> builder,
            Action<IDbCommand> postProcessor,
            Func<IDataRecord, T> mapper,
            CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupAsyncCommand(con, transaction, sql, commandType, commandTimeout))
            {
                builder?.Invoke(cmd);

                try
                {
                    if (wasClosed)
                    {
                        await OpenAsync(con, cancel).ConfigureAwait(false);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync(wasClosed ? CommandBehaviorQueryFirstOrDefaultWithClose : CommandBehaviorQueryFirstOrDefault, cancel).ConfigureAwait(false))
                    {
                        wasClosed = false;

                        postProcessor?.Invoke(cmd);

                        return await reader.ReadAsync(cancel).ConfigureAwait(false) ? mapper(reader) : default;
                    }
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
    }
}
