namespace Smart.Data.Accessor.Benchmark
{
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    using Dapper;

    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Engine;
    using Smart.Mock.Data;

    public static class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<AccessorBenchmark>();
        }
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(MarkdownExporter.Default, MarkdownExporter.GitHub);
            Add(MemoryDiagnoser.Default);
            //Add(Job.LongRun);
            Add(Job.MediumRun);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Ignore")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
    [Config(typeof(BenchmarkConfig))]
    public class AccessorBenchmark
    {
        private MockRepeatDbConnection mockQueryFirst2A;
        private MockRepeatDbConnection mockQueryFirst2B;
        private MockRepeatDbConnection mockQueryFirst4A;
        private MockRepeatDbConnection mockQueryFirst4B;
        private MockRepeatDbConnection mockQueryFirst8A;
        private MockRepeatDbConnection mockQueryFirst8B;
        private MockRepeatDbConnection mockQueryFirst16A;
        private MockRepeatDbConnection mockQueryFirst16B;
        private MockRepeatDbConnection mockQueryFirst32A;
        private MockRepeatDbConnection mockQueryFirst32B;

        private IBenchmarkAccessorForDapper dapperExecuteAccessor;
        private IBenchmarkAccessor smartExecuteAccessor;

        [GlobalSetup]
        public void Setup()
        {
            mockQueryFirst2A = CreateA(2);
            mockQueryFirst2B = CreateB(2);
            mockQueryFirst4A = CreateA(4);
            mockQueryFirst4B = CreateB(4);
            mockQueryFirst8A = CreateA(8);
            mockQueryFirst8B = CreateB(8);
            mockQueryFirst16A = CreateA(16);
            mockQueryFirst16B = CreateB(16);
            mockQueryFirst32A = CreateA(32);
            mockQueryFirst32B = CreateB(32);

            // DAO
            dapperExecuteAccessor = new DapperAccessor();

            var engine = new ExecuteEngineConfig()
                .ToEngine();
            var factory = new DataAccessorFactory(engine);
            smartExecuteAccessor = factory.Create<IBenchmarkAccessor>();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            // 省略
        }

        private static MockRepeatDbConnection CreateA(int n)
        {
            var columns = Enumerable.Range(1, n).Select(x => new MockColumn(typeof(long), $"Id{x}")).ToArray();
            var values = Enumerable.Range(1, n).Select(x => (object)(long)x).ToArray();
            return new MockRepeatDbConnection(new MockDataReader(columns, new[] { values }));
        }

        private static MockRepeatDbConnection CreateB(int n)
        {
            var columns = Enumerable.Range(1, n).Select(x => new MockColumn(typeof(string), $"Name{x}")).ToArray();
            var values = Enumerable.Range(1, n).Select(x => (object)"test").ToArray();
            return new MockRepeatDbConnection(new MockDataReader(columns, new[] { values }));
        }

        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        //[Benchmark]
        //public LongDataEntity DapperQueryFirstOrDefault2A() => dapperExecuteAccessor.QueryFirstOrDefault2A(mockQueryFirst2A, 1);

        //[Benchmark]
        //public LongDataEntity SmartQueryFirstOrDefault2A() => smartExecuteAccessor.QueryFirstOrDefault2A(mockQueryFirst2A, 1);

        //[Benchmark]
        //public StringDataEntity DapperQueryFirstOrDefault2B() => dapperExecuteAccessor.QueryFirstOrDefault2B(mockQueryFirst2B, 1);

        //[Benchmark]
        //public StringDataEntity SmartQueryFirstOrDefault2B() => smartExecuteAccessor.QueryFirstOrDefault2B(mockQueryFirst2B, 1);

        //[Benchmark]
        //public LongDataEntity DapperQueryFirstOrDefault4A() => dapperExecuteAccessor.QueryFirstOrDefault4A(mockQueryFirst4A, 1);

        //[Benchmark]
        //public LongDataEntity SmartQueryFirstOrDefault4A() => smartExecuteAccessor.QueryFirstOrDefault4A(mockQueryFirst4A, 1);

        //[Benchmark]
        //public StringDataEntity DapperQueryFirstOrDefault4B() => dapperExecuteAccessor.QueryFirstOrDefault4B(mockQueryFirst4B, 1);

        //[Benchmark]
        //public StringDataEntity SmartQueryFirstOrDefault4B() => smartExecuteAccessor.QueryFirstOrDefault4B(mockQueryFirst4B, 1);

        //[Benchmark]
        //public LongDataEntity DapperQueryFirstOrDefault8A() => dapperExecuteAccessor.QueryFirstOrDefault8A(mockQueryFirst8A, 1);

        //[Benchmark]
        //public LongDataEntity SmartQueryFirstOrDefault8A() => smartExecuteAccessor.QueryFirstOrDefault8A(mockQueryFirst8A, 1);

        //[Benchmark]
        //public StringDataEntity DapperQueryFirstOrDefault8B() => dapperExecuteAccessor.QueryFirstOrDefault8B(mockQueryFirst8B, 1);

        //[Benchmark]
        //public StringDataEntity SmartQueryFirstOrDefault8B() => smartExecuteAccessor.QueryFirstOrDefault8B(mockQueryFirst8B, 1);

        //[Benchmark]
        //public LongDataEntity DapperQueryFirstOrDefault16A() => dapperExecuteAccessor.QueryFirstOrDefault16A(mockQueryFirst16A, 1);

        //[Benchmark]
        //public LongDataEntity SmartQueryFirstOrDefault16A() => smartExecuteAccessor.QueryFirstOrDefault16A(mockQueryFirst16A, 1);

        //[Benchmark]
        //public StringDataEntity DapperQueryFirstOrDefault16B() => dapperExecuteAccessor.QueryFirstOrDefault16B(mockQueryFirst16B, 1);

        //[Benchmark]
        //public StringDataEntity SmartQueryFirstOrDefault16B() => smartExecuteAccessor.QueryFirstOrDefault16B(mockQueryFirst16B, 1);

        //[Benchmark]
        //public LongDataEntity DapperQueryFirstOrDefault32A() => dapperExecuteAccessor.QueryFirstOrDefault32A(mockQueryFirst32A, 1);

        //[Benchmark]
        //public LongDataEntity SmartQueryFirstOrDefault32A() => smartExecuteAccessor.QueryFirstOrDefault32A(mockQueryFirst32A, 1);

        [Benchmark]
        public StringDataEntity DapperQueryFirstOrDefault32B() => dapperExecuteAccessor.QueryFirstOrDefault32B(mockQueryFirst32B, 1);

        [Benchmark]
        public StringDataEntity SmartQueryFirstOrDefault32B() => smartExecuteAccessor.QueryFirstOrDefault32B(mockQueryFirst32B, 1);
    }

    [DataAccessor]
    public interface IBenchmarkAccessor
    {
        [DirectSql(CommandType.Text, MethodType.QueryFirstOrDefault, "SELECT * FROM Data WHERE Id = /*@ id */0")]
        LongDataEntity QueryFirstOrDefault2A(DbConnection con, long id);

        [DirectSql(CommandType.Text, MethodType.QueryFirstOrDefault, "SELECT * FROM Data WHERE Id = /*@ id */0")]
        StringDataEntity QueryFirstOrDefault2B(DbConnection con, long id);

        [DirectSql(CommandType.Text, MethodType.QueryFirstOrDefault, "SELECT * FROM Data WHERE Id = /*@ id */0")]
        LongDataEntity QueryFirstOrDefault4A(DbConnection con, long id);

        [DirectSql(CommandType.Text, MethodType.QueryFirstOrDefault, "SELECT * FROM Data WHERE Id = /*@ id */0")]
        StringDataEntity QueryFirstOrDefault4B(DbConnection con, long id);

        [DirectSql(CommandType.Text, MethodType.QueryFirstOrDefault, "SELECT * FROM Data WHERE Id = /*@ id */0")]
        LongDataEntity QueryFirstOrDefault8A(DbConnection con, long id);

        [DirectSql(CommandType.Text, MethodType.QueryFirstOrDefault, "SELECT * FROM Data WHERE Id = /*@ id */0")]
        StringDataEntity QueryFirstOrDefault8B(DbConnection con, long id);

        [DirectSql(CommandType.Text, MethodType.QueryFirstOrDefault, "SELECT * FROM Data WHERE Id = /*@ id */0")]
        LongDataEntity QueryFirstOrDefault16A(DbConnection con, long id);

        [DirectSql(CommandType.Text, MethodType.QueryFirstOrDefault, "SELECT * FROM Data WHERE Id = /*@ id */0")]
        StringDataEntity QueryFirstOrDefault16B(DbConnection con, long id);

        [DirectSql(CommandType.Text, MethodType.QueryFirstOrDefault, "SELECT * FROM Data WHERE Id = /*@ id */0")]
        LongDataEntity QueryFirstOrDefault32A(DbConnection con, long id);

        [DirectSql(CommandType.Text, MethodType.QueryFirstOrDefault, "SELECT * FROM Data WHERE Id = /*@ id */0")]
        StringDataEntity QueryFirstOrDefault32B(DbConnection con, long id);
    }

    public interface IBenchmarkAccessorForDapper
    {
        LongDataEntity QueryFirstOrDefault2A(DbConnection con, long id);

        StringDataEntity QueryFirstOrDefault2B(DbConnection con, long id);

        LongDataEntity QueryFirstOrDefault4A(DbConnection con, long id);

        StringDataEntity QueryFirstOrDefault4B(DbConnection con, long id);

        LongDataEntity QueryFirstOrDefault8A(DbConnection con, long id);

        StringDataEntity QueryFirstOrDefault8B(DbConnection con, long id);

        LongDataEntity QueryFirstOrDefault16A(DbConnection con, long id);

        StringDataEntity QueryFirstOrDefault16B(DbConnection con, long id);

        LongDataEntity QueryFirstOrDefault32A(DbConnection con, long id);

        StringDataEntity QueryFirstOrDefault32B(DbConnection con, long id);
    }

    public sealed class DapperAccessor : IBenchmarkAccessorForDapper
    {
        public LongDataEntity QueryFirstOrDefault2A(DbConnection con, long id)
            => con.QueryFirstOrDefault<LongDataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = id });

        public StringDataEntity QueryFirstOrDefault2B(DbConnection con, long id)
            => con.QueryFirstOrDefault<StringDataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = id });

        public LongDataEntity QueryFirstOrDefault4A(DbConnection con, long id)
            => con.QueryFirstOrDefault<LongDataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = id });

        public StringDataEntity QueryFirstOrDefault4B(DbConnection con, long id)
            => con.QueryFirstOrDefault<StringDataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = id });

        public LongDataEntity QueryFirstOrDefault8A(DbConnection con, long id)
            => con.QueryFirstOrDefault<LongDataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = id });

        public StringDataEntity QueryFirstOrDefault8B(DbConnection con, long id)
            => con.QueryFirstOrDefault<StringDataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = id });

        public LongDataEntity QueryFirstOrDefault16A(DbConnection con, long id)
            => con.QueryFirstOrDefault<LongDataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = id });

        public StringDataEntity QueryFirstOrDefault16B(DbConnection con, long id)
            => con.QueryFirstOrDefault<StringDataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = id });

        public LongDataEntity QueryFirstOrDefault32A(DbConnection con, long id)
            => con.QueryFirstOrDefault<LongDataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = id });

        public StringDataEntity QueryFirstOrDefault32B(DbConnection con, long id)
            => con.QueryFirstOrDefault<StringDataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = id });
    }

    public class LongDataEntity
    {
        public long Id1 { get; set; }
        public long Id2 { get; set; }
        public long Id3 { get; set; }
        public long Id4 { get; set; }
        public long Id5 { get; set; }
        public long Id6 { get; set; }
        public long Id7 { get; set; }
        public long Id8 { get; set; }
        public long Id9 { get; set; }
        public long Id10 { get; set; }
        public long Id11 { get; set; }
        public long Id12 { get; set; }
        public long Id13 { get; set; }
        public long Id14 { get; set; }
        public long Id15 { get; set; }
        public long Id16 { get; set; }
        public long Id17 { get; set; }
        public long Id18 { get; set; }
        public long Id19 { get; set; }
        public long Id20 { get; set; }
        public long Id21 { get; set; }
        public long Id22 { get; set; }
        public long Id23 { get; set; }
        public long Id24 { get; set; }
        public long Id25 { get; set; }
        public long Id26 { get; set; }
        public long Id27 { get; set; }
        public long Id28 { get; set; }
        public long Id29 { get; set; }
        public long Id30 { get; set; }
        public long Id31 { get; set; }
        public long Id32 { get; set; }
    }

    public class StringDataEntity
    {
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Name4 { get; set; }
        public string Name5 { get; set; }
        public string Name6 { get; set; }
        public string Name7 { get; set; }
        public string Name8 { get; set; }
        public string Name9 { get; set; }
        public string Name10 { get; set; }
        public string Name11 { get; set; }
        public string Name12 { get; set; }
        public string Name13 { get; set; }
        public string Name14 { get; set; }
        public string Name15 { get; set; }
        public string Name16 { get; set; }
        public string Name17 { get; set; }
        public string Name18 { get; set; }
        public string Name19 { get; set; }
        public string Name20 { get; set; }
        public string Name21 { get; set; }
        public string Name22 { get; set; }
        public string Name23 { get; set; }
        public string Name24 { get; set; }
        public string Name25 { get; set; }
        public string Name26 { get; set; }
        public string Name27 { get; set; }
        public string Name28 { get; set; }
        public string Name29 { get; set; }
        public string Name30 { get; set; }
        public string Name31 { get; set; }
        public string Name32 { get; set; }
    }
}
