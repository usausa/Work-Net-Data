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
        private readonly Action<DbCommand, string, StringBuilder, string[]> setup25_1;
        private readonly string name25_1;

        private readonly Action<DbCommand, string, object> setup26_1;
        private readonly Func<DbCommand, string, object, DbParameter> setup26_2;
        private readonly Func<DbCommand, string, DbParameter> setup26_3;
        private readonly string name26_1;
        private readonly string name26_2;
        private readonly string name26_3;

        private readonly Action<DbCommand, string, object> setup27_1;
        private readonly Func<DbCommand, string, object, DbParameter> setup27_2;
        private readonly Func<DbCommand, string, DbParameter> setup27_3;
        // ReSharper restore InconsistentNaming

        // TODO(out専用)、なくす代わりに事前評価済みのconverterを取得する、統一？
        private readonly MockTypeHandler mockTypeHandler;

        // TODO ダイレクトかエンジン経由かで切り分ける？
        // TODO (IQueryExtension(E), ITypeHandler(E), DbType(E))[Engine作成？] | IParameterBuilder(直作成？)
        // TODO 直Convert<ITypeHandler(E)を含んでる>[Engine作成？] | IResultParser(直作成？)

        // TODO TypeHandler(Parse)は直接こちらに定義、この情報はEngineにある！★
        // TODO Readerは直接こちらに定義
        // TODO 上記を踏まえるとParameterAction相当(ResultFunction)も型にまかせる？
        // TODO engine.Convert直の禁止、ターゲット方はTypedになっている点

        public SampleDao(ExecuteEngine engine)
        {
            this.engine = engine;
            provider = engine.GetComponent<IDbProvider>();

            var method25 = GetType().GetMethod("ExecuteEnumerable");
            setup25_1 = engine.CreateArrayParameterSetup<string>(typeof(string), method25.GetParameters()[0]);
            name25_1 = engine.GetParameterName(0);

            setup26_1 = engine.CreateInParameterSetup(typeof(string), null);
            setup26_2 = engine.CreateInOutParameterSetup(typeof(int), null);
            setup26_3 = engine.CreateOutParameterSetup(ParameterDirection.Output);
            name26_1 = engine.GetParameterName(0);
            name26_2 = engine.GetParameterName(1);
            name26_3 = engine.GetParameterName(2);

            setup27_1 = engine.CreateInParameterSetup(typeof(int), null);
            setup27_2 = engine.CreateInOutParameterSetup(typeof(int), null);
            setup27_3 = engine.CreateOutParameterSetup(ParameterDirection.Output);

            mockTypeHandler = engine.GetTypeHandler<MockTypeHandler>();
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

                var result = engine.ExecuteScalar<long>(cmd);

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

                var result = await engine.ExecuteScalarAsync<long>(cmd, cancel).ConfigureAwait(false);

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
                var result = engine.ExecuteScalar<long>(con, cmd);

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
                var result = await engine.ExecuteScalarAsync<long>(con, cmd, cancel).ConfigureAwait(false);

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
                var _outParam1 = default(DbParameter);
                var _outParam2 = default(DbParameter);   // [MEMO] コード的には冗長だが

                // parameter.InParam
                if (ScriptHelper.IsEmpty(parameter.InParam))
                {
                    setup26_1(_cmd, name26_1, parameter.InParam);
                }

                // parameter.InOutParam
                if (ScriptHelper.IsNotNull(parameter.InOutParam))
                {
                    _outParam1 = setup26_2(_cmd, name26_2, parameter.InOutParam);
                }

                // parameter.OutParam
                _outParam2 = setup26_3(_cmd, name26_3);

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
                    parameter.InOutParam = engine.Convert<int?>(_outParam1.Value);
                }

                if (_outParam2 != null)
                {
                    parameter.OutParam = engine.Convert<int>(_outParam2.Value);
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
                var _outParam1 = default(DbParameter);
                var _outParam2 = default(DbParameter);   // [MEMO] コード的には冗長だが

                // param1
                setup27_1(_cmd, engine.GetParameterName(_nameIndex++), param1);

                // param2
                _outParam1 = setup27_2(_cmd, engine.GetParameterName(_nameIndex++), param2);

                // param3
                _outParam2 = setup27_3(_cmd, engine.GetParameterName(_nameIndex++));

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
                    param3 = engine.Convert<int>(_outParam2.Value);
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
