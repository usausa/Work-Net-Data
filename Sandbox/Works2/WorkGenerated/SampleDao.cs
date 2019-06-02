using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkGenerated
{
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

        // TODO Get
        private readonly MockTypeHandler mockTypeHandler = new MockTypeHandler();

        // TODO Dynamic
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
                var result = ExecuteHelper.Execute(con, cmd);

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
                var result = await ExecuteHelper.ExecuteAsync(con, cmd, cancel).ConfigureAwait(false);

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

                var result = ExecuteHelper.ExecuteScalar<long>(cmd);

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

                var result = await ExecuteHelper.ExecuteScalarAsync<long>(cmd, cancel).ConfigureAwait(false);

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
                var result = ExecuteHelper.ExecuteScalar<long>(con, cmd);

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
                var result = await ExecuteHelper.ExecuteScalarAsync<long>(con, cmd, cancel).ConfigureAwait(false);

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

                reader = ExecuteHelper.ExecuteReader(cmd);

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

                reader = await ExecuteHelper.ExecuteReaderAsync(cmd, cancel).ConfigureAwait(false);

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

                reader = ExecuteHelper.ExecuteReader(cmd, wasClosed);
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

                reader = await ExecuteHelper.ExecuteReaderAsync(cmd, wasClosed, cancel).ConfigureAwait(false);
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

                reader = ExecuteHelper.ExecuteReader(cmd);

                // Post action

                return ExecuteHelper.ReaderToDefer(cmd, reader, mapperDataEntity);
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

                reader = await ExecuteHelper.ExecuteReaderAsync(cmd, cancel).ConfigureAwait(false);

                // Post action

                return ExecuteHelper.ReaderToDefer(cmd, reader, mapperDataEntity);
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

                reader = ExecuteHelper.ExecuteReader(cmd, wasClosed);
                wasClosed = false;

                // Post action

                return ExecuteHelper.ReaderToDefer(cmd, reader, mapperDataEntity);
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

                reader = await ExecuteHelper.ExecuteReaderAsync(cmd, wasClosed, cancel).ConfigureAwait(false);
                wasClosed = false;

                // Post action

                return ExecuteHelper.ReaderToDefer(cmd, reader, mapperDataEntity);
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

                var list = ExecuteHelper.QueryBuffer(cmd, mapperDataEntity);

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

                var list = await ExecuteHelper.QueryBufferAsync(cmd, mapperDataEntity, cancel).ConfigureAwait(false);

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
                var list = ExecuteHelper.QueryBuffer(con, cmd, mapperDataEntity);

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
                var list = await ExecuteHelper.QueryBufferAsync(con, cmd, mapperDataEntity, cancel).ConfigureAwait(false);

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

                var result = ExecuteHelper.QueryFirstOrDefault(cmd, mapperDataEntity);

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

                var result = await ExecuteHelper.QueryFirstOrDefaultAsync(cmd, mapperDataEntity, cancel).ConfigureAwait(false);

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
                var result = ExecuteHelper.QueryFirstOrDefault(con, cmd, mapperDataEntity);

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
                var result = await ExecuteHelper.QueryFirstOrDefaultAsync(con, cmd, mapperDataEntity, cancel).ConfigureAwait(false);

                // Post action

                return result;
            }
        }

        //--------------------------------------------------------------------------------
        // Enumerable
        //--------------------------------------------------------------------------------

        // TODO enumerable
        // TODO enumerable with dynamic parameter

        // ReSharper disable RedundantAssignment
        // ReSharper disable InconsistentNaming
        public int ExecuteEnumerable(string[] ids)
        {
            using (var _con = provider.CreateConnection())
            using (var _cmd = _con.CreateCommand())
            {
                var _sql = new StringBuilder(128);

                _sql.Append("SELECT * FROM Test ");

                if (ids.Length > 0)
                {
                    _sql.Append("WHERE Id IN ");

                    InClauseHelper.AddParameter(_sql, _cmd, "p1", DbType.AnsiString, 3, ids);
                }

                // Build command
                _cmd.CommandText = _sql.ToString();

                // Execute
                _con.Open();

                var _result = _cmd.ExecuteNonQuery();

                return _result;
            }
        }

        //--------------------------------------------------------------------------------
        // Full
        //--------------------------------------------------------------------------------

        // ReSharper disable RedundantAssignment
        // ReSharper disable InconsistentNaming
        public int ExecuteEx1(ProcParameter parameter, int timeout)
        {
            using (var _con = provider.CreateConnection())
            using (var _cmd = _con.CreateCommand())
            {
                // [MEMO] Direction.Returnは引数で扱えない
                var _outParam1 = default(DbParameter);
                var _outParam2 = default(DbParameter);   // [MEMO] コード的には冗長だが

                // parameter.InParam
                if (SqlHelper.IsEmpty(parameter.InParam))
                {
                    DbCommandHelper.AddParameterWithDirection(
                        _cmd, "p1", ParameterDirection.Input, DbType.AnsiString, 5, parameter.InParam);
                }

                // parameter.InOutParam
                if (SqlHelper.IsNotNull(parameter.InOutParam))
                {
                    _outParam1 = DbCommandHelper.AddParameterWithDirectionAndReturn(
                        _cmd, "p2", ParameterDirection.InputOutput, DbType.Int32, parameter.InOutParam);
                }

                // parameter.OutParam
                _outParam2 = DbCommandHelper.AddParameterWithDirectionAndReturn(
                    _cmd, "p3", ParameterDirection.Output, DbType.Int32, parameter.OutParam);

                // Build command
                _cmd.CommandText = "PROC";
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = timeout;

                // Execute
                _con.Open();

                var _result = _cmd.ExecuteNonQuery();

                // Post action
                // [MEMO] Dynamicでなければnullチェックが不要
                if (_outParam1 != null)
                {
                    parameter.InOutParam = ConvertHelper.Convert<int?>(_outParam1.Value);
                }

                if (_outParam2 != null)
                {
                    parameter.OutParam = ConvertHelper.Convert<int>(_outParam2.Value);
                }

                return _result;
            }
        }

        public void ExecuteEx2(int param1, ref int param2, out int param3)
        {
            using (var _con = provider.CreateConnection())
            using (var _cmd = _con.CreateCommand())
            {
                // [MEMO] Direction.Returnは引数で扱えない
                var _outParam1 = default(DbParameter);
                var _outParam2 = default(DbParameter);   // [MEMO] コード的には冗長だが

                // param1
                DbCommandHelper.AddParameterWithDirection(
                    _cmd, "p1", ParameterDirection.Input, DbType.Int32,param1);

                // param2
                _outParam1 = DbCommandHelper.AddParameterWithDirectionAndReturn(
                    _cmd, "p2", ParameterDirection.InputOutput, mockTypeHandler, param2);

                // param3
                _outParam2 = DbCommandHelper.AddParameterWithDirectionAndReturn(
                    _cmd, "p3", ParameterDirection.ReturnValue, DbType.Int32);

                // Build command
                _cmd.CommandText = "PROC";
                _cmd.CommandType = CommandType.StoredProcedure;

                // Execute
                _con.Open();

                // [MEMO] 戻り値未使用
                _cmd.ExecuteNonQuery();

                // Post action
                // [MEMO] Dynamicでなければnullチェックが不要
                if (_outParam1 != null)
                {
                    param2 = (int)mockTypeHandler.Parse(typeof(int), _outParam1.Value);
                }

                if (_outParam2 != null)
                {
                    param3 = ConvertHelper.Convert<int>(_outParam2.Value);
                }
                else
                {
                    param3 = default;
                }
            }
        }
        // ReSharper restore InconsistentNaming
        // ReSharper restore RedundantAssignment
    }
}
