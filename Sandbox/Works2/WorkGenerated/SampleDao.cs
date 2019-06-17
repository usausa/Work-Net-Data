using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataLibrary.Attributes;
using DataLibrary.Engine;
using DataLibrary.Helper;
using DataLibrary.Providers;

namespace WorkGenerated
{
    // MEMO コネクションが引数の場合はOpenしていることが条件
    // MEMO DbConnectionならキャストなし
    // MEMO DbTransactionが引数ならコマンドに設定
    // MEMO DbTransactionのみの場合はそこからコマンドを作成
    // MEMO Asyncメソッドを作れるのはDbConnectionが引数の時だけ？

    // MEMO post処理、outがある場合のみ、なければ呼び出しも省略形！

    // MEMO cmdの設定、tx(opt)、type(opt)、text、timeout(opt)、and parameters

    public class SampleDao
    {
        private readonly ExecuteEngine engine;

        // TODO(cast?)
        private readonly IDbProvider provider;

        // ReSharper disable InconsistentNaming
        public Func<object, object> converter5;
        public Func<object, object> converter6;
        public Func<object, object> converter7;
        public Func<object, object> converter8;

        private readonly Action<DbCommand, string, StringBuilder, string[]> setup25_1;
        private readonly string name25_1;

        private readonly string name26_1;
        private readonly string name26_2;
        private readonly string name26_3;
        private readonly Action<DbCommand, string, object> setup26_1;
        private readonly Func<DbCommand, string, object, DbParameter> setup26_2;
        private readonly Func<DbCommand, string, DbParameter> setup26_3;
        public Func<object, object> converter26_2;
        public Func<object, object> converter26_3;

        private readonly Action<DbCommand, string, object> setup27_1;
        private readonly Func<DbCommand, string, object, DbParameter> setup27_2;
        private readonly Func<DbCommand, string, DbParameter> setup27_3;
        public Func<object, object> converter27_2;
        public Func<object, object> converter27_3;
        // ReSharper restore InconsistentNaming

        public SampleDao(ExecuteEngine engine)
        {
            this.engine = engine;
            provider = engine.GetComponent<IDbProvider>();

            var method5 = GetType().GetMethod("ExecuteScalar", Type.EmptyTypes);
            converter5 = engine.CreateConverter<long>(method5.ReturnParameter);

            var method6 = GetType().GetMethod("ExecuteScalarAsync", new [] { typeof(CancellationToken) });
            converter6 = engine.CreateConverter<long>(method6.ReturnParameter);

            var method7 = GetType().GetMethod("ExecuteScalar", new[] { typeof(DbConnection) });
            converter7 = engine.CreateConverter<long>(method7.ReturnParameter);

            var method8 = GetType().GetMethod("ExecuteScalarAsync", new[] { typeof(DbConnection), typeof(CancellationToken) });
            converter8 = engine.CreateConverter<long>(method8.ReturnParameter);

            var method25 = GetType().GetMethod("ExecuteEnumerable", new[] { typeof(string[]) });
            setup25_1 = engine.CreateArrayParameterSetup<string>(method25.GetParameters()[0]);
            name25_1 = engine.GetParameterName(0);

            var method26 = GetType().GetMethod("ExecuteEx1", new[] { typeof(ProcParameter), typeof(int) });
            name26_1 = engine.GetParameterName(0);
            name26_2 = engine.GetParameterName(1);
            name26_3 = engine.GetParameterName(2);
            setup26_1 = engine.CreateInParameterSetup<string>(null);
            setup26_2 = engine.CreateInOutParameterSetup<int?>(null);
            setup26_3 = engine.CreateOutParameterSetup(ParameterDirection.Output);
            converter26_2 = engine.CreateConverter<int?>(method26.GetParameters()[0].ParameterType.GetProperty("InParam"));
            converter26_3 = engine.CreateConverter<int>(method26.GetParameters()[0].ParameterType.GetProperty("InOutParam"));

            var method27 = GetType().GetMethod("ExecuteEx2", new[] { typeof(int), typeof(int).MakeByRefType(), typeof(int).MakeByRefType() });
            setup27_1 = engine.CreateInParameterSetup<int>(null);
            setup27_2 = engine.CreateInOutParameterSetup<int>(null);
            setup27_3 = engine.CreateOutParameterSetup(ParameterDirection.Output);
            converter27_2 = engine.CreateConverter<int>(method27.GetParameters()[1]);
            converter27_3 = engine.CreateConverter<int>(method27.GetParameters()[2]);
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
                var result = engine.Execute(con, cmd);

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
                var result = await engine.ExecuteAsync(con, cmd, cancel).ConfigureAwait(false);

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

                var result = ConvertHelper.Convert<long>(
                    engine.ExecuteScalar(cmd),
                    converter5);

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

                var result = ConvertHelper.Convert<long>(
                    await engine.ExecuteScalarAsync(cmd, cancel).ConfigureAwait(false),
                    converter6);

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
                var result = ConvertHelper.Convert<long>(
                    engine.ExecuteScalar(con, cmd),
                    converter7);

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
                var result = ConvertHelper.Convert<long>(
                    await engine.ExecuteScalarAsync(con, cmd, cancel).ConfigureAwait(false),
                    converter8);

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

                reader = engine.ExecuteReader(cmd);

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

                reader = await engine.ExecuteReaderAsync(cmd, cancel).ConfigureAwait(false);

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

                reader = engine.ExecuteReader(cmd, wasClosed);
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

                reader = await engine.ExecuteReaderAsync(cmd, wasClosed, cancel).ConfigureAwait(false);
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

                reader = engine.ExecuteReader(cmd);

                // Post action

                return engine.ReaderToDefer<DataEntity>(cmd, reader);
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

                reader = await engine.ExecuteReaderAsync(cmd, cancel).ConfigureAwait(false);

                // Post action

                return engine.ReaderToDefer<DataEntity>(cmd, reader);
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

                reader = engine.ExecuteReader(cmd, wasClosed);
                wasClosed = false;

                // Post action

                return engine.ReaderToDefer<DataEntity>(cmd, reader);
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

                reader = await engine.ExecuteReaderAsync(cmd, wasClosed, cancel).ConfigureAwait(false);
                wasClosed = false;

                // Post action

                return engine.ReaderToDefer<DataEntity>(cmd, reader);
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

                var list = engine.QueryBuffer<DataEntity>(cmd);

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

                var list = await engine.QueryBufferAsync<DataEntity>(cmd, cancel).ConfigureAwait(false);

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
                var list = engine.QueryBuffer<DataEntity>(con, cmd);

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
                var list = await engine.QueryBufferAsync<DataEntity>(con, cmd, cancel).ConfigureAwait(false);

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

                var result = engine.QueryFirstOrDefault<DataEntity>(cmd);

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

                var result = await engine.QueryFirstOrDefaultAsync<DataEntity>(cmd, cancel).ConfigureAwait(false);

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
                var result = engine.QueryFirstOrDefault<DataEntity>(con, cmd);

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
                var result = await engine.QueryFirstOrDefaultAsync<DataEntity>(con, cmd, cancel).ConfigureAwait(false);

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
        public int ExecuteEnumerable([AnsiString(3)] string[] ids)
        {
            using (var _con = provider.CreateConnection())
            using (var _cmd = _con.CreateCommand())
            {
                var _sql = new StringBuilder(128);

                _sql.Append("SELECT * FROM Test ");

                if (ids.Length > 0)
                {
                    _sql.Append("WHERE Id IN ");

                    setup25_1(_cmd, name25_1, _sql, ids);
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
                var _outParam2 = default(DbParameter);
                var _outParam3 = default(DbParameter);   // [MEMO] コード的には冗長だが

                // parameter.InParam
                if (ScriptHelper.IsEmpty(parameter.InParam))
                {
                    setup26_1(_cmd, name26_1, parameter.InParam);
                }

                // parameter.InOutParam
                if (ScriptHelper.IsNotNull(parameter.InOutParam))
                {
                    _outParam2 = setup26_2(_cmd, name26_2, parameter.InOutParam);
                }

                // parameter.OutParam
                _outParam3 = setup26_3(_cmd, name26_3);

                // Build command
                _cmd.CommandText = "PROC";
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = timeout;

                // Execute
                _con.Open();

                var _result = _cmd.ExecuteNonQuery();

                // Post action
                // [MEMO] Dynamicでなければnullチェックが不要
                if (_outParam2 != null)
                {
                    parameter.InOutParam = ConvertHelper.Convert<int?>(_outParam2.Value, converter26_2);
                }

                if (_outParam3 != null)
                {
                    parameter.OutParam = ConvertHelper.Convert<int>(_outParam3.Value, converter26_3);
                }

                return _result;
            }
        }

        public void ExecuteEx2(int param1, ref int param2, out int param3)
        {
            using (var _con = provider.CreateConnection())
            using (var _cmd = _con.CreateCommand())
            {
                // [MEMO] Dynamicなら名前も動的
                var _nameIndex = 0;

                // [MEMO] Direction.Returnは引数で扱えない
                var _outParam2 = default(DbParameter);
                var _outParam3 = default(DbParameter);   // [MEMO] コード的には冗長だが

                // param1
                setup27_1(_cmd, engine.GetParameterName(_nameIndex++), param1);

                // param2
                _outParam2 = setup27_2(_cmd, engine.GetParameterName(_nameIndex++), param2);

                // param3
                _outParam3 = setup27_3(_cmd, engine.GetParameterName(_nameIndex++));

                // Build command
                _cmd.CommandText = "PROC";
                _cmd.CommandType = CommandType.StoredProcedure;

                // Execute
                _con.Open();

                // [MEMO] 戻り値未使用
                _cmd.ExecuteNonQuery();

                // Post action
                // [MEMO] Dynamicでなければnullチェックが不要
                if (_outParam2 != null)
                {
                    param2 = ConvertHelper.Convert<int>(_outParam2.Value, converter26_2);
                }

                if (_outParam3 != null)
                {
                    param3 = ConvertHelper.Convert<int>(_outParam3.Value, converter26_3);
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
