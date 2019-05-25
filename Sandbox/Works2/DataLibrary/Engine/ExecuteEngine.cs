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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Execute(IDbConnection con, DbCommand cmd)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            try
            {
                if (wasClosed)
                {
                    con.Open();
                }

                return cmd.ExecuteNonQuery();
            }
            finally
            {
                if (wasClosed)
                {
                    con.Close();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<int> ExecuteAsync(IDbConnection con, DbCommand cmd, CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            try
            {
                if (wasClosed)
                {
                    await OpenAsync(con, cancel).ConfigureAwait(false);
                }

                return await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);
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
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ExecuteScalar<T>(IDbConnection con, DbCommand cmd, Func<object, object> converter)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            try
            {
                if (wasClosed)
                {
                    con.Open();
                }

                var result = cmd.ExecuteScalar();

                if (result is DBNull)
                {
                    return default;
                }

                if (result is T scalar)
                {
                    return scalar;
                }

                return (T)converter(result);
            }
            finally
            {
                if (wasClosed)
                {
                    con.Close();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> ExecuteScalarAsync<T>(IDbConnection con, DbCommand cmd, Func<object, object> converter, CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            try
            {
                if (wasClosed)
                {
                    await OpenAsync(con, cancel).ConfigureAwait(false);
                }

                var result = await cmd.ExecuteScalarAsync(cancel).ConfigureAwait(false);

                if (result is DBNull)
                {
                    return default;
                }

                if (result is T scalar)
                {
                    return scalar;
                }

                return (T)converter(result);
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
        // ExecuteReader
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDataReader ExecuteReader(IDbConnection con, DbCommand cmd, CommandBehavior commandBehavior)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var reader = default(IDataReader);
            try
            {
                if (wasClosed)
                {
                    con.Open();
                }

                reader = cmd.ExecuteReader(wasClosed
                    ? commandBehavior | CommandBehavior.CloseConnection
                    : commandBehavior);
                wasClosed = false;

                return new WrappedReader(cmd, reader);
            }
            catch (Exception)
            {
                reader?.Dispose();
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<IDataReader> ExecuteReaderAsync(IDbConnection con, DbCommand cmd, CommandBehavior commandBehavior, CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var reader = default(IDataReader);
            try
            {
                if (wasClosed)
                {
                    await OpenAsync(con, cancel).ConfigureAwait(false);
                }

                reader = await cmd.ExecuteReaderAsync(wasClosed ? commandBehavior | CommandBehavior.CloseConnection : commandBehavior, cancel).ConfigureAwait(false);
                wasClosed = false;

                return new WrappedReader(cmd, reader);
            }
            catch (Exception)
            {
                reader?.Dispose();
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Query<T>(IDbConnection con, DbCommand cmd, bool buffered, Func<IDataRecord, T> mapper)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var reader = default(IDataReader);
            try
            {
                if (wasClosed)
                {
                    con.Open();
                }

                reader = cmd.ExecuteReader(wasClosed ? CommandBehaviorQueryWithClose : CommandBehaviorQuery);
                wasClosed = false;

                if (buffered)
                {
                    return ReaderToList(reader, mapper);
                }
                else
                {
                    var deferred = ReaderToDefer(cmd, reader, mapper);
                    reader = null;
                    return deferred;
                }
            }
            finally
            {
                reader?.Dispose();

                if (wasClosed)
                {
                    con.Close();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection con, DbCommand cmd, bool buffered, Func<IDataRecord, T> mapper, CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var reader = default(DbDataReader);
            try
            {
                if (wasClosed)
                {
                    await OpenAsync(con, cancel).ConfigureAwait(false);
                }

                reader = await cmd.ExecuteReaderAsync(wasClosed ? CommandBehaviorQueryWithClose : CommandBehaviorQuery, cancel).ConfigureAwait(false);
                wasClosed = false;

                if (buffered)
                {
                    return ReaderToList(reader, mapper);
                }
                else
                {
                    var deferred = ReaderToDefer(cmd, reader, mapper);
                    reader = null;
                    return deferred;
                }
            }
            finally
            {
                reader?.Dispose();

                if (wasClosed)
                {
                    con.Close();
                }
            }
        }

        //--------------------------------------------------------------------------------
        // QueryFirstOrDefault
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T QueryFirstOrDefault<T>(IDbConnection con, DbCommand cmd, Func<IDataRecord, T> mapper)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            try
            {
                if (wasClosed)
                {
                    con.Open();
                }

                using (var reader = cmd.ExecuteReader(wasClosed ? CommandBehaviorQueryFirstOrDefaultWithClose : CommandBehaviorQueryFirstOrDefault))
                {
                    wasClosed = false;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> QueryFirstOrDefaultAsync<T>(IDbConnection con, DbCommand cmd, Func<IDataRecord, T> mapper, CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            try
            {
                if (wasClosed)
                {
                    await OpenAsync(con, cancel).ConfigureAwait(false);
                }

                using (var reader = await cmd.ExecuteReaderAsync(wasClosed ? CommandBehaviorQueryFirstOrDefaultWithClose : CommandBehaviorQueryFirstOrDefault, cancel).ConfigureAwait(false))
                {
                    wasClosed = false;
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
