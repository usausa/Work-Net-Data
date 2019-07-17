namespace WorkGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Smart.ComponentModel;
    using Smart.Data.Accessor.Attributes;

    public class MyAttribute : ResultParserAttribute
    {
        public override Func<object, object> CreateConverter(IServiceProvider serviceProvider, Type type)
        {
            return x => x;
        }
    }

    //[NamedProvider("")]
    [Dao]
    public interface IFullSpecDao
    {
        //[NamedProvider("")]
        [Execute] int Execute();
        [Execute] Task<int> ExecuteAsync(CancellationToken cancel);
        [Execute] int Execute(DbConnection con);
        [Execute] Task<int> ExecuteAsync(DbConnection con, CancellationToken cancel);
        [ExecuteScalar] object ExecuteScalarObject();
        [ExecuteScalar] Task<object> ExecuteScalarObjectAsync();
        //[My]
        [ExecuteScalar] long ExecuteScalar();
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
        //[Query] List<DataEntity> QueryBuffer2();
        //[Query] Task<List<DataEntity>> QueryBuffer2Async(CancellationToken cancel);
        //[Query] List<DataEntity> QueryBuffer2(DbConnection con);
        //[Query] Task<List<DataEntity>> QueryBuffer2Async(DbConnection con, CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault();
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(CancellationToken cancel);
        [QueryFirstOrDefault] DataEntity QueryFirstOrDefault(DbConnection con);
        [QueryFirstOrDefault] Task<DataEntity> QueryFirstOrDefaultAsync(DbConnection con, CancellationToken cancel);
        [Execute] int ExecuteEnumerable([AnsiString(3)] string[] ids);
        [Procedure("TEST1")] int ExecuteEx1(ProcParameter parameter, [TimeoutParameter] int timeout);
        [Procedure("TEST2")] void ExecuteEx2(int param1, ref int param2, out int param3);
        //// TODO Provider, Timeout, Tx only ?
    }

    [Dao]
    public interface IMiscDao
    {
        [Execute]
        int Execute(int id, string name);

        [ExecuteScalar]
        long Count();

        [ExecuteScalar]
        long Count1(string name, string code);

        [ExecuteScalar]
        long Count2(string code);

        [Execute]
        int ExecuteIn1(string[] ids);

        [Execute]
        int ExecuteIn2(IList<string> ids);

        [Query]
        IList<DataEntity> QueryDataList(string name, string code, string order);

        [Query]
        IList<DataEntity> QueryDataList2(QueryParameter parameter);

        [Insert("Data")]
        void Insert(DataEntity entity);

        [Procedure("PROC1")]
        void Call1(ProcParameter parameter);

        [Procedure("PROC2")]
        int Call2(int param1, ref int param2, out int param3);
    }
}
