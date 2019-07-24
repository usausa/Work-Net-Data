namespace WorkPerformance
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Smart.Data.Accessor.Attributes;

    [Dao]
    public interface IFullSpecDao01
    {
        [Execute] int Execute();
        [Execute] Task<int> ExecuteAsync(CancellationToken cancel);
        [Execute] int Execute(DbConnection con);
        [Execute] Task<int> ExecuteAsync(DbConnection con, CancellationToken cancel);
        [ExecuteScalar] object ExecuteScalarObject();
        [ExecuteScalar] Task<object> ExecuteScalarObjectAsync();
        [ExecuteScalar] long ExecuteScalar();
        [ExecuteScalar] Task<long> ExecuteScalarAsync(CancellationToken cancel);
        [ExecuteScalar] long ExecuteScalar(DbConnection con);
        [ExecuteScalar] Task<long> ExecuteScalarAsync(DbConnection con, CancellationToken cancel);
        [ExecuteReader] IDataReader ExecuteReader();
        [ExecuteReader] Task<IDataReader> ExecuteReaderAsync(CancellationToken cancel);
        [ExecuteReader] IDataReader ExecuteReader(DbConnection con);
        [ExecuteReader] Task<IDataReader> ExecuteReaderAsync(DbConnection con, CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer();
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer(DbConnection con);
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(DbConnection con, CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer();
        [Query] Task<IList<DataEntity>> QueryBufferAsync(CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer(DbConnection con);
        [Query] Task<IList<DataEntity>> QueryBufferAsync(DbConnection con, CancellationToken cancel);
        [Query] List<DataEntity> QueryBuffer2();
        [Query] Task<List<DataEntity>> QueryBuffer2Async(CancellationToken cancel);
        [Query] List<DataEntity> QueryBuffer2(DbConnection con);
        [Query] Task<List<DataEntity>> QueryBuffer2Async(DbConnection con, CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault();
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault(DbConnection con);
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(DbConnection con, CancellationToken cancel);
        [Execute] int ExecuteEnumerable([AnsiString(3)] string[] ids);
        [Procedure("TEST1")] int ExecuteEx1(ProcParameter parameter, [TimeoutParameter] int timeout);
        [Procedure("TEST2")] void ExecuteEx2(int param1, ref int param2, out int param3);
    }

    [Dao]
    public interface IFullSpecDao02
    {
        [Execute] int Execute();
        [Execute] Task<int> ExecuteAsync(CancellationToken cancel);
        [Execute] int Execute(DbConnection con);
        [Execute] Task<int> ExecuteAsync(DbConnection con, CancellationToken cancel);
        [ExecuteScalar] object ExecuteScalarObject();
        [ExecuteScalar] Task<object> ExecuteScalarObjectAsync();
        [ExecuteScalar] long ExecuteScalar();
        [ExecuteScalar] Task<long> ExecuteScalarAsync(CancellationToken cancel);
        [ExecuteScalar] long ExecuteScalar(DbConnection con);
        [ExecuteScalar] Task<long> ExecuteScalarAsync(DbConnection con, CancellationToken cancel);
        [ExecuteReader] IDataReader ExecuteReader();
        [ExecuteReader] Task<IDataReader> ExecuteReaderAsync(CancellationToken cancel);
        [ExecuteReader] IDataReader ExecuteReader(DbConnection con);
        [ExecuteReader] Task<IDataReader> ExecuteReaderAsync(DbConnection con, CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer();
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer(DbConnection con);
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(DbConnection con, CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer();
        [Query] Task<IList<DataEntity>> QueryBufferAsync(CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer(DbConnection con);
        [Query] Task<IList<DataEntity>> QueryBufferAsync(DbConnection con, CancellationToken cancel);
        [Query] List<DataEntity> QueryBuffer2();
        [Query] Task<List<DataEntity>> QueryBuffer2Async(CancellationToken cancel);
        [Query] List<DataEntity> QueryBuffer2(DbConnection con);
        [Query] Task<List<DataEntity>> QueryBuffer2Async(DbConnection con, CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault();
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault(DbConnection con);
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(DbConnection con, CancellationToken cancel);
        [Execute] int ExecuteEnumerable([AnsiString(3)] string[] ids);
        [Procedure("TEST1")] int ExecuteEx1(ProcParameter parameter, [TimeoutParameter] int timeout);
        [Procedure("TEST2")] void ExecuteEx2(int param1, ref int param2, out int param3);
    }

    [Dao]
    public interface IFullSpecDao03
    {
        [Execute] int Execute();
        [Execute] Task<int> ExecuteAsync(CancellationToken cancel);
        [Execute] int Execute(DbConnection con);
        [Execute] Task<int> ExecuteAsync(DbConnection con, CancellationToken cancel);
        [ExecuteScalar] object ExecuteScalarObject();
        [ExecuteScalar] Task<object> ExecuteScalarObjectAsync();
        [ExecuteScalar] long ExecuteScalar();
        [ExecuteScalar] Task<long> ExecuteScalarAsync(CancellationToken cancel);
        [ExecuteScalar] long ExecuteScalar(DbConnection con);
        [ExecuteScalar] Task<long> ExecuteScalarAsync(DbConnection con, CancellationToken cancel);
        [ExecuteReader] IDataReader ExecuteReader();
        [ExecuteReader] Task<IDataReader> ExecuteReaderAsync(CancellationToken cancel);
        [ExecuteReader] IDataReader ExecuteReader(DbConnection con);
        [ExecuteReader] Task<IDataReader> ExecuteReaderAsync(DbConnection con, CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer();
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer(DbConnection con);
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(DbConnection con, CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer();
        [Query] Task<IList<DataEntity>> QueryBufferAsync(CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer(DbConnection con);
        [Query] Task<IList<DataEntity>> QueryBufferAsync(DbConnection con, CancellationToken cancel);
        [Query] List<DataEntity> QueryBuffer2();
        [Query] Task<List<DataEntity>> QueryBuffer2Async(CancellationToken cancel);
        [Query] List<DataEntity> QueryBuffer2(DbConnection con);
        [Query] Task<List<DataEntity>> QueryBuffer2Async(DbConnection con, CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault();
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault(DbConnection con);
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(DbConnection con, CancellationToken cancel);
        [Execute] int ExecuteEnumerable([AnsiString(3)] string[] ids);
        [Procedure("TEST1")] int ExecuteEx1(ProcParameter parameter, [TimeoutParameter] int timeout);
        [Procedure("TEST2")] void ExecuteEx2(int param1, ref int param2, out int param3);
    }

    [Dao]
    public interface IFullSpecDao04
    {
        [Execute] int Execute();
        [Execute] Task<int> ExecuteAsync(CancellationToken cancel);
        [Execute] int Execute(DbConnection con);
        [Execute] Task<int> ExecuteAsync(DbConnection con, CancellationToken cancel);
        [ExecuteScalar] object ExecuteScalarObject();
        [ExecuteScalar] Task<object> ExecuteScalarObjectAsync();
        [ExecuteScalar] long ExecuteScalar();
        [ExecuteScalar] Task<long> ExecuteScalarAsync(CancellationToken cancel);
        [ExecuteScalar] long ExecuteScalar(DbConnection con);
        [ExecuteScalar] Task<long> ExecuteScalarAsync(DbConnection con, CancellationToken cancel);
        [ExecuteReader] IDataReader ExecuteReader();
        [ExecuteReader] Task<IDataReader> ExecuteReaderAsync(CancellationToken cancel);
        [ExecuteReader] IDataReader ExecuteReader(DbConnection con);
        [ExecuteReader] Task<IDataReader> ExecuteReaderAsync(DbConnection con, CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer();
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer(DbConnection con);
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(DbConnection con, CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer();
        [Query] Task<IList<DataEntity>> QueryBufferAsync(CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer(DbConnection con);
        [Query] Task<IList<DataEntity>> QueryBufferAsync(DbConnection con, CancellationToken cancel);
        [Query] List<DataEntity> QueryBuffer2();
        [Query] Task<List<DataEntity>> QueryBuffer2Async(CancellationToken cancel);
        [Query] List<DataEntity> QueryBuffer2(DbConnection con);
        [Query] Task<List<DataEntity>> QueryBuffer2Async(DbConnection con, CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault();
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault(DbConnection con);
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(DbConnection con, CancellationToken cancel);
        [Execute] int ExecuteEnumerable([AnsiString(3)] string[] ids);
        [Procedure("TEST1")] int ExecuteEx1(ProcParameter parameter, [TimeoutParameter] int timeout);
        [Procedure("TEST2")] void ExecuteEx2(int param1, ref int param2, out int param3);
    }

    [Dao]
    public interface IFullSpecDao05
    {
        [Execute] int Execute();
        [Execute] Task<int> ExecuteAsync(CancellationToken cancel);
        [Execute] int Execute(DbConnection con);
        [Execute] Task<int> ExecuteAsync(DbConnection con, CancellationToken cancel);
        [ExecuteScalar] object ExecuteScalarObject();
        [ExecuteScalar] Task<object> ExecuteScalarObjectAsync();
        [ExecuteScalar] long ExecuteScalar();
        [ExecuteScalar] Task<long> ExecuteScalarAsync(CancellationToken cancel);
        [ExecuteScalar] long ExecuteScalar(DbConnection con);
        [ExecuteScalar] Task<long> ExecuteScalarAsync(DbConnection con, CancellationToken cancel);
        [ExecuteReader] IDataReader ExecuteReader();
        [ExecuteReader] Task<IDataReader> ExecuteReaderAsync(CancellationToken cancel);
        [ExecuteReader] IDataReader ExecuteReader(DbConnection con);
        [ExecuteReader] Task<IDataReader> ExecuteReaderAsync(DbConnection con, CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer();
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer(DbConnection con);
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(DbConnection con, CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer();
        [Query] Task<IList<DataEntity>> QueryBufferAsync(CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer(DbConnection con);
        [Query] Task<IList<DataEntity>> QueryBufferAsync(DbConnection con, CancellationToken cancel);
        [Query] List<DataEntity> QueryBuffer2();
        [Query] Task<List<DataEntity>> QueryBuffer2Async(CancellationToken cancel);
        [Query] List<DataEntity> QueryBuffer2(DbConnection con);
        [Query] Task<List<DataEntity>> QueryBuffer2Async(DbConnection con, CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault();
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault(DbConnection con);
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(DbConnection con, CancellationToken cancel);
        [Execute] int ExecuteEnumerable([AnsiString(3)] string[] ids);
        [Procedure("TEST1")] int ExecuteEx1(ProcParameter parameter, [TimeoutParameter] int timeout);
        [Procedure("TEST2")] void ExecuteEx2(int param1, ref int param2, out int param3);
    }
}
