using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WorkGenerated
{
    public static class DaoHelper
    {
        public const CommandBehavior CommandBehaviorForEnumerable =
            CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection;

        public const CommandBehavior CommandBehaviorForList =
            CommandBehavior.SequentialAccess;

        public const CommandBehavior CommandBehaviorForSingle =
            CommandBehavior.SequentialAccess | CommandBehavior.SingleRow;

        // TODO Execute *2

        // TODO Scalar helper *2

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<T> ReaderToList<T>(IDataReader reader, Func<IDataReader, T> mapper)
        {
            var list = new List<T>();
            while (reader.Read())
            {
                list.Add(mapper(reader));
            }

            return list;
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

        // MEMO 今はConverterの利用を検討
        private readonly Func<object, object> converter;

        private readonly Func<IDataRecord, DataEntity> mapperDataEntity;

        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        public int Execute()
        {
            // Connection
            using (var con = provider.CreateConnection())
            {
                // Command
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Data (Id, Name) VALUES (1, 'test')";

                    // Execute
                    con.Open();

                    var result = cmd.ExecuteNonQuery();

                    // Post action

                    return result;
                }
            }
        }

        // With Cancel
        public async Task<int> ExecuteAsync(CancellationToken cancel)
        {
            // Connection
            using (var con = provider.CreateConnection())
            {
                // Command
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Data (Id, Name) VALUES (1, 'test')";

                    // Execute
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    var result = await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);

                    // Post action

                    return result;
                }
            }
        }

        // With Connection
        public int Execute(DbConnection con)
        {
            // Command
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Data (Id, Name) VALUES (1, 'test')";

                // TODO con open ?
                // Execute
                var result = cmd.ExecuteNonQuery();

                // Post action

                return result;
            }
        }

        // With Connection, Cancel
        public async Task<int> ExecuteAsync(DbConnection con, CancellationToken cancel)
        {
            // Command
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Data (Id, Name) VALUES (1, 'test')";

                // TODO con open ?
                // Execute
                var result = await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);

                // Post action

                return result;
            }
        }

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        public long ExecuteScalar()
        {
            // Connection
            using (var con = provider.CreateConnection())
            {
                // Command
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Data";

                    // Execute
                    con.Open();

                    var result = cmd.ExecuteScalar();

                    // Post action

                    if (result is DBNull)
                    {
                        return default;
                    }

                    if (result is long scalar)
                    {
                        return scalar;
                    }

                    return (long)converter(result);
                }
            }
        }

        // With Cancel
        public async Task<long> ExecuteScalarAsync(CancellationToken cancel)
        {
            // Connection
            using (var con = provider.CreateConnection())
            {
                // Command
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Data";

                    // Execute
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    var result = await cmd.ExecuteScalarAsync(cancel).ConfigureAwait(false);

                    // Post action

                    if (result is DBNull)
                    {
                        return default;
                    }

                    if (result is long scalar)
                    {
                        return scalar;
                    }

                    return (long)converter(result);
                }
            }
        }

        // With Connection
        public long ExecuteScalar(DbConnection con)
        {
            // Command
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Data";

                // TODO con open ?
                // Execute
                var result = cmd.ExecuteScalar();

                // Post action

                if (result is DBNull)
                {
                    return default;
                }

                if (result is long scalar)
                {
                    return scalar;
                }

                return (long)converter(result);
            }
        }

        // With Connection, Cancel
        public async Task<long> ExecuteScalarAsync(DbConnection con, CancellationToken cancel)
        {
            // Command
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Data";

                // TODO con open ?
                // Execute
                var result = await cmd.ExecuteScalarAsync(cancel);

                // Post action

                if (result is DBNull)
                {
                    return default;
                }

                if (result is long scalar)
                {
                    return scalar;
                }

                return (long)converter(result);
            }
        }

        //--------------------------------------------------------------------------------
        // ExecuteReader
        //--------------------------------------------------------------------------------

        public IDataReader ExecuteReader()
        {
            // TODO con ?
            // Connection
            using (var con = provider.CreateConnection())
            {
                var reader = default(IDataReader);
                var cmd = con.CreateCommand();
                try
                {
                    cmd.CommandText = "SELECT * FROM Data";

                    // Execute
                    con.Open();

                    reader = cmd.ExecuteReader(DaoHelper.CommandBehaviorForEnumerable);

                    // Post action

                    return new WrappedReader(cmd, reader);
                }
                catch (Exception)
                {
                    reader?.Dispose();
                    cmd.Dispose();
                    throw;
                }
            }
        }

        // With Cancel
        public async Task<IDataReader> ExecuteReaderAsync(CancellationToken cancel)
        {
            // TODO con ?
            // Connection
            using (var con = provider.CreateConnection())
            {
                var reader = default(IDataReader);
                var cmd = con.CreateCommand();
                try
                {
                    cmd.CommandText = "SELECT * FROM Data";

                    // Execute
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    reader = await cmd.ExecuteReaderAsync(DaoHelper.CommandBehaviorForEnumerable, cancel).ConfigureAwait(false);

                    // Post action

                    return new WrappedReader(cmd, reader);
                }
                catch (Exception)
                {
                    reader?.Dispose();
                    cmd.Dispose();
                    throw;
                }
            }
        }

        // With Connection
        public IDataReader ExecuteReader(DbConnection con)
        {
            var reader = default(IDataReader);
            var cmd = con.CreateCommand();
            try
            {
                cmd.CommandText = "SELECT * FROM Data";

                // TODO con open ?(try統合)
                // Execute
                reader = cmd.ExecuteReader(DaoHelper.CommandBehaviorForEnumerable);

                // Post action

                return new WrappedReader(cmd, reader);
            }
            catch (Exception)
            {
                reader?.Dispose();
                cmd.Dispose();
                throw;
            }
        }

        // With Connection, Cancel
        public async Task<IDataReader> ExecuteReaderAsync(DbConnection con, CancellationToken cancel)
        {
            var reader = default(IDataReader);
            var cmd = con.CreateCommand();
            try
            {
                cmd.CommandText = "SELECT * FROM Data";

                // TODO con open ?(try統合)
                // Execute
                reader = await cmd.ExecuteReaderAsync(DaoHelper.CommandBehaviorForEnumerable, cancel).ConfigureAwait(false);

                // Post action

                return new WrappedReader(cmd, reader);
            }
            catch (Exception)
            {
                reader?.Dispose();
                cmd.Dispose();
                throw;
            }
        }

        //--------------------------------------------------------------------------------
        // Query(Buffered)
        //--------------------------------------------------------------------------------

        public IList<DataEntity> QueryBuffer()
        {
            // Connection
            using (var con = provider.CreateConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                    // Execute
                    con.Open();

                    using (var reader = cmd.ExecuteReader(DaoHelper.CommandBehaviorForList))
                    {
                        // Post action

                        return DaoHelper.ReaderToList(reader, mapperDataEntity);
                    }
                }
            }
        }

        // With Cancel
        public async Task<IList<DataEntity>> QueryBufferAsync(CancellationToken cancel)
        {
            // Connection
            using (var con = provider.CreateConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    using (var reader = await cmd.ExecuteReaderAsync(DaoHelper.CommandBehaviorForList, cancel))
                    {
                        // Post action

                        return DaoHelper.ReaderToList(reader, mapperDataEntity);
                    }
                }
            }
        }

        // With Connection
        public IList<DataEntity> QueryBuffer(DbConnection con)
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                if (con.State == ConnectionState.Closed)
                {
                    try
                    {
                        con.Open();

                        // Execute
                        using (var reader = cmd.ExecuteReader(DaoHelper.CommandBehaviorForList))
                        {
                            // Post action

                            return DaoHelper.ReaderToList(reader, mapperDataEntity);
                        }
                    }
                    finally
                    {
                        con.Close();
                    }
                }
                else
                {
                    // Execute
                    using (var reader = cmd.ExecuteReader(DaoHelper.CommandBehaviorForList))
                    {
                        // Post action

                        return DaoHelper.ReaderToList(reader, mapperDataEntity);
                    }
                }
            }
        }

        // With Connection, Cancel
        public async Task<IList<DataEntity>> QueryBufferAsync(DbConnection con, CancellationToken cancel)
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                if (con.State == ConnectionState.Closed)
                {
                    try
                    {
                        await con.OpenAsync(cancel).ConfigureAwait(false);

                        // Execute
                        using (var reader = await cmd.ExecuteReaderAsync(DaoHelper.CommandBehaviorForList, cancel))
                        {
                            // Post action

                            return DaoHelper.ReaderToList(reader, mapperDataEntity);
                        }
                    }
                    finally
                    {
                        con.Close();
                    }
                }
                else
                {
                    // Execute
                    using (var reader = await cmd.ExecuteReaderAsync(DaoHelper.CommandBehaviorForList, cancel))
                    {
                        // Post action

                        return DaoHelper.ReaderToList(reader, mapperDataEntity);
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------
        // Query(NonBuffered)
        //--------------------------------------------------------------------------------

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

        public IEnumerable<DataEntity> QueryNonBuffer()
        {
            var close = true;
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            var con = provider.CreateConnection();
            try
            {
                cmd = con.CreateCommand();

                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                con.Open();

                reader = cmd.ExecuteReader(DaoHelper.CommandBehaviorForEnumerable);
                close = false;

                // Post action

                var deferred = ReaderToDefer(cmd, reader, mapperDataEntity);
                cmd = null;
                reader = null;
                return deferred;
            }
            finally
            {
                reader?.Dispose();
                cmd?.Dispose();
                if (close)
                {
                    con.Close();
                }
            }
        }

        public async Task<IEnumerable<DataEntity>> QueryNonBufferAsync(CancellationToken cancel)
        {
            var close = true;
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            var con = provider.CreateConnection();
            try
            {
                cmd = con.CreateCommand();

                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                await con.OpenAsync(cancel).ConfigureAwait(false);

                reader = await cmd.ExecuteReaderAsync(DaoHelper.CommandBehaviorForEnumerable, cancel).ConfigureAwait(false);
                close = false;

                // Post action

                var deferred = ReaderToDefer(cmd, reader, mapperDataEntity);
                cmd = null;
                reader = null;
                return deferred;
            }
            finally
            {
                reader?.Dispose();
                cmd?.Dispose();
                if (close)
                {
                    con.Close();
                }
            }
        }

        public IEnumerable<DataEntity> QueryNonBuffer(DbConnection con)
        {
            var close = con.State == ConnectionState.Closed;
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            try
            {
                cmd = con.CreateCommand();

                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                if (close)
                {
                    con.Open();
                }

                reader = cmd.ExecuteReader(DaoHelper.CommandBehaviorForEnumerable);
                close = false;

                // Post action

                var deferred = ReaderToDefer(cmd, reader, mapperDataEntity);
                cmd = null;
                reader = null;
                return deferred;
            }
            finally
            {
                reader?.Dispose();
                cmd?.Dispose();
                if (close)
                {
                    con.Close();
                }
            }
        }

        public async Task<IEnumerable<DataEntity>> QueryNonBufferAsync(DbConnection con, CancellationToken cancel)
        {
            var close = con.State == ConnectionState.Closed;
            var reader = default(IDataReader);
            var cmd = default(DbCommand);
            try
            {
                cmd = con.CreateCommand();

                cmd.CommandText = "SELECT * FROM Data";

                // Execute
                if (close)
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);
                }

                reader = await cmd.ExecuteReaderAsync(DaoHelper.CommandBehaviorForEnumerable, cancel).ConfigureAwait(false);
                close = false;

                // Post action

                var deferred = ReaderToDefer(cmd, reader, mapperDataEntity);
                cmd = null;
                reader = null;
                return deferred;
            }
            finally
            {
                reader?.Dispose();
                cmd?.Dispose();
                if (close)
                {
                    con.Close();
                }
            }
        }

        //--------------------------------------------------------------------------------
        // QueryFirstOrDefault
        //--------------------------------------------------------------------------------

        public DataEntity QueryFirstOrDefault()
        {
            // Connection
            using (var con = provider.CreateConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                    // Execute
                    con.Open();

                    using (var reader = cmd.ExecuteReader(DaoHelper.CommandBehaviorForSingle))
                    {
                        // Post action

                        return reader.Read() ? mapperDataEntity(reader) : default;
                    }
                }
            }
        }

        // With Cancel
        public async Task<DataEntity> QueryFirstOrDefault(CancellationToken cancel)
        {
            // Connection
            using (var con = provider.CreateConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                    // Execute
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    using (var reader = await cmd.ExecuteReaderAsync(DaoHelper.CommandBehaviorForSingle, cancel).ConfigureAwait(false))
                    {
                        // Post action

                        return reader.Read() ? mapperDataEntity(reader) : default;
                    }
                }
            }
        }

        // With Connection
        public DataEntity QueryFirstOrDefault(DbConnection con)
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                // Execute
                using (var reader = cmd.ExecuteReader(DaoHelper.CommandBehaviorForSingle))
                {
                    // Post action

                    return reader.Read() ? mapperDataEntity(reader) : default;
                }
            }
        }

        // With Connection, Cancel
        public async Task<DataEntity> QueryFirstOrDefault(DbConnection con, CancellationToken cancel)
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Data WHERE Id = 1";

                // Execute
                using (var reader = await cmd.ExecuteReaderAsync(DaoHelper.CommandBehaviorForSingle, cancel).ConfigureAwait(false))
                {
                    // Post action

                    return reader.Read() ? mapperDataEntity(reader) : default;
                }
            }
        }

        //--------------------------------------------------------------------------------
        // SpecialMethod
        //--------------------------------------------------------------------------------

        // TODO procedure Executeになるのとパラメタの違い

        // TODO Insert Executeになるだけ

        //--------------------------------------------------------------------------------
        // Option
        //--------------------------------------------------------------------------------

        // TODO 引数にTimeout CMDが違うだけ

        //--------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------

        // TODO optimized 単一SQL

        // TODO conditional basic args1, 2, class

        // TODO enumerable

        // TODO enumerable with dynamic parameter

        // TODO size and dbTye, parameter / class 2*2

        // TODO TypeHandler , parameter / class 1*2

        // TODO out/ref

        // TODO direction

        //--------------------------------------------------------------------------------
        // Full
        //--------------------------------------------------------------------------------

        // TODO mixed?
    }
}
