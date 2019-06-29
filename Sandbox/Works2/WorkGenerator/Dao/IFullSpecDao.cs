using System;
using DataLibrary.Providers;

namespace WorkGenerator.Dao
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    using DataLibrary.Attributes;

    using WorkGenerator.Models;

    public class MyAttribute : ResultParserAttribute
    {
        public override Func<object, object> CreateConverter(Type type)
        {
            return x => x;
        }
    }

    [NamedProvider("")]
    [Dao]
    public interface IFullSpecDao
    {
        [Execute] int Execute();
        [Execute] Task<int> ExecuteAsync(CancellationToken cancel);
        [Execute] int Execute(DbConnection con);
        [Execute] Task<int> ExecuteAsync(DbConnection con, CancellationToken cancel);
        [ExecuteScalar][My] long ExecuteScalar();
        [ExecuteScalar] Task<long> ExecuteScalarAsync(CancellationToken cancel);
        [ExecuteScalar] long ExecuteScalar(DbConnection con);
        [ExecuteScalar] Task<long> ExecuteScalarAsync(DbConnection con, CancellationToken cancel);
        [ExecuteScalarReader] IDataReader ExecuteReader();
        [ExecuteScalarReader] Task<IDataReader> ExecuteReaderAsync(CancellationToken cancel);
        [ExecuteScalarReader] IDataReader ExecuteReader(DbConnection con);
        [ExecuteScalarReader] Task<IDataReader> ExecuteReaderAsync(DbConnection con, CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer();
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(CancellationToken cancel);
        [Query] IEnumerable<DataEntity> QueryNonBuffer(DbConnection con);
        [Query] Task<IEnumerable<DataEntity>> QueryNonBufferAsync(DbConnection con, CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer();
        [Query] Task<IList<DataEntity>> QueryBufferAsync(CancellationToken cancel);
        [Query] IList<DataEntity> QueryBuffer(DbConnection con);
        [Query] Task<IList<DataEntity>> QueryBufferAsync(DbConnection con, CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault();
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault(DbConnection con);
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(DbConnection con, CancellationToken cancel);
        [Execute] int ExecuteEnumerable([AnsiString(3)] string[] ids);
        [Execute] int ExecuteEx1(ProcParameter parameter, [TimeoutParameter] int timeout);
        [Execute] void ExecuteEx2(int param1, ref int param2, out int param3);
        // TODO Provider, Timeout, Tx only ?
    }
}
