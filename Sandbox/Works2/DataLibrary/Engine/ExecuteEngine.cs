namespace DataLibrary.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ExecuteEngine
    {
        private const CommandBehavior CommandBehaviorForEnumerable =
            CommandBehavior.SequentialAccess;

        private const CommandBehavior CommandBehaviorForEnumerableWithClose =
            CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection;

        private const CommandBehavior CommandBehaviorForList =
            CommandBehavior.SequentialAccess;

        private const CommandBehavior CommandBehaviorForSingle =
            CommandBehavior.SequentialAccess | CommandBehavior.SingleRow;

        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Execute(DbConnection con, DbCommand cmd)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    con.Open();

                    return cmd.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }

            return cmd.ExecuteNonQuery();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<int> ExecuteAsync(DbConnection con, DbCommand cmd, CancellationToken cancel = default)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    return await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);
                }
                finally
                {
                    con.Close();
                }
            }

            return await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);
        }

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ExecuteScalar<T>(DbCommand cmd)
        {
            var result = cmd.ExecuteScalar();

            if (result is T scalar)
            {
                return scalar;
            }

            if (result is DBNull)
            {
                return default;
            }

            return (T)Convert.ChangeType(result, typeof(T), CultureInfo.InvariantCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> ExecuteScalarAsync<T>(DbCommand cmd, CancellationToken cancel = default)
        {
            var result = await cmd.ExecuteScalarAsync(cancel).ConfigureAwait(false);

            if (result is T scalar)
            {
                return scalar;
            }

            if (result is DBNull)
            {
                return default;
            }

            return (T)Convert.ChangeType(result, typeof(T), CultureInfo.InvariantCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ExecuteScalar<T>(DbConnection con, DbCommand cmd)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    con.Open();

                    return ExecuteScalar<T>(cmd);
                }
                finally
                {
                    con.Close();
                }
            }

            return ExecuteScalar<T>(cmd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> ExecuteScalarAsync<T>(DbConnection con, DbCommand cmd, CancellationToken cancel = default)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    return await ExecuteScalarAsync<T>(cmd, cancel).ConfigureAwait(false);
                }
                finally
                {
                    con.Close();
                }
            }

            return await ExecuteScalarAsync<T>(cmd, cancel).ConfigureAwait(false);
        }

        //--------------------------------------------------------------------------------
        // ExecuteReader
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DbDataReader ExecuteReader(DbCommand cmd)
        {
            return cmd.ExecuteReader(CommandBehaviorForEnumerableWithClose);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<DbDataReader> ExecuteReaderAsync(DbCommand cmd, CancellationToken cancel)
        {
            return cmd.ExecuteReaderAsync(CommandBehaviorForEnumerableWithClose, cancel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DbDataReader ExecuteReader(DbCommand cmd, bool withClose)
        {
            return cmd.ExecuteReader(withClose ? CommandBehaviorForEnumerableWithClose : CommandBehaviorForEnumerable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<DbDataReader> ExecuteReaderAsync(DbCommand cmd, bool withClose, CancellationToken cancel)
        {
            return cmd.ExecuteReaderAsync(withClose ? CommandBehaviorForEnumerableWithClose : CommandBehaviorForEnumerable, cancel);
        }

        //--------------------------------------------------------------------------------
        // ReaderToDefer
        //--------------------------------------------------------------------------------

        public static IEnumerable<T> ReaderToDefer<T>(IDbCommand cmd, IDataReader reader, Func<IDataRecord, T> mapper)
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

        //--------------------------------------------------------------------------------
        // QueryBuffer
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<T> QueryBuffer<T>(DbCommand cmd, Func<IDataReader, T> mapper)
        {
            using (var reader = cmd.ExecuteReader(CommandBehaviorForList))
            {
                var list = new List<T>();
                while (reader.Read())
                {
                    list.Add(mapper(reader));
                }

                return list;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<IList<T>> QueryBufferAsync<T>(DbCommand cmd, Func<IDataReader, T> mapper, CancellationToken cancel = default)
        {
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehaviorForList, cancel).ConfigureAwait(false))
            {
                var list = new List<T>();
                while (reader.Read())
                {
                    list.Add(mapper(reader));
                }

                return list;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<T> QueryBuffer<T>(DbConnection con, DbCommand cmd, Func<IDataReader, T> mapper)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    con.Open();

                    return QueryBuffer(cmd, mapper);
                }
                finally
                {
                    con.Close();
                }
            }


            return QueryBuffer(cmd, mapper);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<IList<T>> QueryBufferAsync<T>(DbConnection con, DbCommand cmd, Func<IDataReader, T> mapper, CancellationToken cancel = default)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    return await QueryBufferAsync(cmd, mapper, cancel).ConfigureAwait(false);
                }
                finally
                {
                    con.Close();
                }
            }

            return await QueryBufferAsync(cmd, mapper, cancel).ConfigureAwait(false);
        }

        //--------------------------------------------------------------------------------
        // QueryFirstOrDefault
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T QueryFirstOrDefault<T>(DbCommand cmd, Func<IDataReader, T> mapper)
        {
            using (var reader = cmd.ExecuteReader(CommandBehaviorForSingle))
            {
                return reader.Read() ? mapper(reader) : default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> QueryFirstOrDefaultAsync<T>(DbCommand cmd, Func<IDataReader, T> mapper, CancellationToken cancel = default)
        {
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehaviorForSingle, cancel))
            {
                return reader.Read() ? mapper(reader) : default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T QueryFirstOrDefault<T>(DbConnection con, DbCommand cmd, Func<IDataReader, T> mapper)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    con.Open();

                    return QueryFirstOrDefault(cmd, mapper);
                }
                finally
                {
                    con.Close();
                }
            }

            return QueryFirstOrDefault(cmd, mapper);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> QueryFirstOrDefaultAsync<T>(DbConnection con, DbCommand cmd, Func<IDataReader, T> mapper, CancellationToken cancel = default)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    return await QueryFirstOrDefaultAsync(cmd, mapper, cancel).ConfigureAwait(false);
                }
                finally
                {
                    con.Close();
                }
            }

            return await QueryFirstOrDefaultAsync(cmd, mapper, cancel).ConfigureAwait(false);
        }
    }
}
