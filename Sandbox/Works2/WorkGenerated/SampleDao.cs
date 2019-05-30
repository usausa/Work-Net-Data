using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WorkGenerated
{
    public static class DaoHelper
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

    // MEMO コンストラクタはComponentProviderのみ
    // MEMO Namedプロバイダーの処理はコンストラクタ

    // MEMO コネクションが引数の場合はOpenしていることが条件
    // MEMO DbConnectionならキャストなし
    // MEMO DbTransactionが引数ならコマンドに設定
    // MEMO DbTransactionのみの場合はそこからコマンドを作成
    // MEMO Asyncメソッドを作れるのはDbConnectionが引数の時だけ？

    // MEMO post処理、outがある場合のみ、なければ呼び出しも省略形！

    // MEMO cmdの設定、tx(opt)、type(opt)、text、timeout(opt)、and parameters

    public class SampleDao
    {
        private readonly IDbProvider provider;

        private readonly Func<IDataRecord, DataEntity> mapperDataEntity;

        public SampleDao(
            IDbProvider provider,
            Func<IDataRecord, DataEntity> mapperDataEntity)
        {
            this.provider = provider;
            this.mapperDataEntity = mapperDataEntity;
        }

        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        public int Execute()
        {
            using (var con = provider.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "INSERT INTO Data (Id, Name) VALUES (1, 'test')";

                // Execute
                con.Open();

                var result = cmd.ExecuteNonQuery();

                // Post action

                return result;
            }
        }

        // With Cancel
        public async Task<int> ExecuteAsync(CancellationToken cancel)
        {
            using (var con = provider.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "INSERT INTO Data (Id, Name) VALUES (1, 'test')";

                // Execute
                await con.OpenAsync(cancel).ConfigureAwait(false);

                var result = await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);

                // Post action

                return result;
            }
        }

        // With Connection
        public int Execute(DbConnection con)
        {
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "INSERT INTO Data (Id, Name) VALUES (1, 'test')";

                // Execute
                var result = DaoHelper.Execute(con, cmd);

                // Post action

                return result;
            }
        }

        // With Connection, Cancel
        public async Task<int> ExecuteAsync(DbConnection con, CancellationToken cancel)
        {
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "INSERT INTO Data (Id, Name) VALUES (1, 'test')";

                // Execute
                var result = await DaoHelper.ExecuteAsync(con, cmd, cancel).ConfigureAwait(false);

                // Post action

                return result;
            }
        }

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        public long ExecuteScalar()
        {
            using (var con = provider.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT COUNT(*) FROM Data";

                // Execute
                con.Open();

                var result = DaoHelper.ExecuteScalar<long>(cmd);

                // Post action

                return result;
            }
        }

        // With Cancel
        public async Task<long> ExecuteScalarAsync(CancellationToken cancel)
        {
            using (var con = provider.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT COUNT(*) FROM Data";

                // Execute
                await con.OpenAsync(cancel).ConfigureAwait(false);

                var result = await DaoHelper.ExecuteScalarAsync<long>(cmd, cancel).ConfigureAwait(false);

                // Post action

                return result;
            }
        }

        // With Connection
        public long ExecuteScalar(DbConnection con)
        {
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT COUNT(*) FROM Data";

                // Execute
                var result = DaoHelper.ExecuteScalar<long>(con, cmd);

                // Post action

                return result;
            }
        }

        // With Connection, Cancel
        public async Task<long> ExecuteScalarAsync(DbConnection con, CancellationToken cancel)
        {
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT COUNT(*) FROM Data";

                // Execute
                var result = await DaoHelper.ExecuteScalarAsync<long>(con, cmd, cancel).ConfigureAwait(false);

                // Post action

                return result;
            }
        }

        //--------------------------------------------------------------------------------
        // ExecuteReader
        //--------------------------------------------------------------------------------

        public IDataReader ExecuteReader()
        {
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            var con = provider.CreateConnection();
            try
            {
                cmd = con.CreateCommand();

                // Build command
                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                con.Open();

                reader = DaoHelper.ExecuteReader(cmd);

                // Post action

                return new WrappedReader(cmd, reader);
            }
            catch (Exception)
            {
                reader?.Dispose();
                cmd?.Dispose();
                con.Close();
                throw;
            }
        }

        // With Cancel
        public async Task<IDataReader> ExecuteReaderAsync(CancellationToken cancel)
        {
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            var con = provider.CreateConnection();
            try
            {
                cmd = con.CreateCommand();

                // Build command
                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                await con.OpenAsync(cancel).ConfigureAwait(false);

                reader = await DaoHelper.ExecuteReaderAsync(cmd, cancel).ConfigureAwait(false);

                // Post action

                return new WrappedReader(cmd, reader);
            }
            catch (Exception)
            {
                reader?.Dispose();
                cmd?.Dispose();
                con.Close();
                throw;
            }
        }

        // With Connection
        public IDataReader ExecuteReader(DbConnection con)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            try
            {
                cmd = con.CreateCommand();

                // Build command
                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                if (wasClosed)
                {
                    con.Open();
                }

                reader = DaoHelper.ExecuteReader(cmd, wasClosed);
                wasClosed = false;

                // Post action

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

        // With Connection, Cancel
        public async Task<IDataReader> ExecuteReaderAsync(DbConnection con, CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            try
            {
                cmd = con.CreateCommand();

                // Build command
                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                if (wasClosed)
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);
                }

                reader = await DaoHelper.ExecuteReaderAsync(cmd, wasClosed, cancel).ConfigureAwait(false);
                wasClosed = false;

                // Post action

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
        // Query(NonBuffered)
        //--------------------------------------------------------------------------------

        public IEnumerable<DataEntity> QueryNonBuffer()
        {
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            var con = provider.CreateConnection();
            try
            {
                cmd = con.CreateCommand();

                // Build command
                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                con.Open();

                reader = DaoHelper.ExecuteReader(cmd);

                // Post action

                return DaoHelper.ReaderToDefer(cmd, reader, mapperDataEntity);
            }
            catch (Exception)
            {
                reader?.Dispose();
                cmd?.Dispose();
                con.Close();
                throw;
            }
        }

        public async Task<IEnumerable<DataEntity>> QueryNonBufferAsync(CancellationToken cancel)
        {
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            var con = provider.CreateConnection();
            try
            {
                cmd = con.CreateCommand();

                // Build command
                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                await con.OpenAsync(cancel).ConfigureAwait(false);

                reader = await DaoHelper.ExecuteReaderAsync(cmd, cancel).ConfigureAwait(false);

                // Post action

                return DaoHelper.ReaderToDefer(cmd, reader, mapperDataEntity);
            }
            catch (Exception)
            {
                reader?.Dispose();
                cmd?.Dispose();
                con.Close();
                throw;
            }
        }

        public IEnumerable<DataEntity> QueryNonBuffer(DbConnection con)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            try
            {
                cmd = con.CreateCommand();

                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                if (wasClosed)
                {
                    con.Open();
                }

                reader = DaoHelper.ExecuteReader(cmd, wasClosed);
                wasClosed = false;

                // Post action

                return DaoHelper.ReaderToDefer(cmd, reader, mapperDataEntity);
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

        public async Task<IEnumerable<DataEntity>> QueryNonBufferAsync(DbConnection con, CancellationToken cancel)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            try
            {
                cmd = con.CreateCommand();

                // Build command
                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                if (wasClosed)
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);
                }

                reader = await DaoHelper.ExecuteReaderAsync(cmd, wasClosed, cancel).ConfigureAwait(false);
                wasClosed = false;

                // Post action

                return DaoHelper.ReaderToDefer(cmd, reader, mapperDataEntity);
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
        // Query(Buffered)
        //--------------------------------------------------------------------------------

        public IList<DataEntity> QueryBuffer()
        {
            using (var con = provider.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                // Execute
                con.Open();

                var list = DaoHelper.QueryBuffer(cmd, mapperDataEntity);

                // Post action

                return list;
            }
        }

        // With Cancel
        public async Task<IList<DataEntity>> QueryBufferAsync(CancellationToken cancel)
        {
            using (var con = provider.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                // Execute
                await con.OpenAsync(cancel).ConfigureAwait(false);

                var list = await DaoHelper.QueryBufferAsync(cmd, mapperDataEntity, cancel).ConfigureAwait(false);

                // Post action

                return list;
            }
        }

        // With Connection
        public IList<DataEntity> QueryBuffer(DbConnection con)
        {
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                // Execute
                var list = DaoHelper.QueryBuffer(con, cmd, mapperDataEntity);

                // Post action

                return list;
            }
        }

        // With Connection, Cancel
        public async Task<IList<DataEntity>> QueryBufferAsync(DbConnection con, CancellationToken cancel)
        {
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                // Execute
                var list = await DaoHelper.QueryBufferAsync(con, cmd, mapperDataEntity, cancel).ConfigureAwait(false);

                // Post action

                return list;
            }
        }

        //--------------------------------------------------------------------------------
        // QueryFirstOrDefault
        //--------------------------------------------------------------------------------

        public DataEntity QueryFirstOrDefault()
        {
            using (var con = provider.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                // Execute
                con.Open();

                var result = DaoHelper.QueryFirstOrDefault(cmd, mapperDataEntity);

                // Post action

                return result;
            }
        }

        // With Cancel
        public async Task<DataEntity> QueryFirstOrDefaultAsync(CancellationToken cancel)
        {
            using (var con = provider.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                // Execute
                await con.OpenAsync(cancel).ConfigureAwait(false);

                var result = await DaoHelper.QueryFirstOrDefaultAsync(cmd, mapperDataEntity, cancel).ConfigureAwait(false);

                // Post action

                return result;
            }
        }

        // With Connection
        public DataEntity QueryFirstOrDefault(DbConnection con)
        {
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                // Execute
                var result = DaoHelper.QueryFirstOrDefault(con, cmd, mapperDataEntity);

                // Post action

                return result;
            }
        }

        // With Connection, Cancel
        public async Task<DataEntity> QueryFirstOrDefaultAsync(DbConnection con, CancellationToken cancel)
        {
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                // Execute
                var result = await DaoHelper.QueryFirstOrDefaultAsync(con, cmd, mapperDataEntity, cancel).ConfigureAwait(false);

                // Post action

                return result;
            }
        }

        //--------------------------------------------------------------------------------
        // SpecialMethod
        //--------------------------------------------------------------------------------

        // [低優先]
        // TODO procedure Executeになるのとパラメタの違い
        // TODO Insert Executeになるだけ

        //--------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------

        // TODO conditional basic args1, 2
        // TODO class

        // TODO enumerable
        // TODO enumerable with dynamic parameter

        //--------------------------------------------------------------------------------
        // Full
        //--------------------------------------------------------------------------------

        // TODO parameter helper

        // TODO size and dbTye, parameter / class 2*2
        // TODO TypeHandler , parameter / class 1*2

        // TODO out/ref
        // TODO direction

        // CommandType
        // Timeout
        public int ExecuteEx(ProcParameter parameter, int timeout)
        {
            using (var con = provider.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                // Build command
                cmd.CommandText = "PROC";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = timeout;

                // Execute
                con.Open();

                var result = cmd.ExecuteNonQuery();

                // Post action

                return result;
            }
        }
    }
}
