namespace WorkBenchmark
{
    using System;
    using System.Data;
    using System.IO;
    using System.Text;

    using Dapper;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    using Microsoft.Data.Sqlite;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark>();
        }
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(MarkdownExporter.Default, MarkdownExporter.GitHub);
            Add(MemoryDiagnoser.Default);
            Add(Job.MediumRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private const string FileConnectionString = "Data Source=bench.db";

        private const string MemoryConnectionString = "Data Source=bench;Mode=memory";

        private IBenchmarkDao manualDao;

        private IBenchmarkDao generatedDao;

        private IBenchmarkDao generatedDao2;

        private IBenchmarkDao memoryManualDao;

        private IBenchmarkDao memoryGeneratedDao;

        private IBenchmarkDao memoryGeneratedDao2;

        private IDbConnection memoryCon;

        [GlobalSetup]
        public void Setup()
        {
            File.Delete("bench.db");
            using (var con = new SqliteConnection(FileConnectionString))
            {
                con.Open();
                PrepareDatabase(con);
            }

            memoryCon = new SqliteConnection(MemoryConnectionString);
            memoryCon.Open();
            PrepareDatabase(memoryCon);

            manualDao = new ManualBenchmarkDao(() => new SqliteConnection(FileConnectionString));
            generatedDao = new GeneratedBenchmarkDao(new DapperExecutor(), new SingleConnectionManager(() => new SqliteConnection(FileConnectionString)));
            generatedDao2 = new GeneratedBenchmarkDao2(new DapperExecutor(), new SingleConnectionManager(() => new SqliteConnection(FileConnectionString)));

            memoryManualDao = new ForMemoryManualBenchmarkDao(() => memoryCon);
            memoryGeneratedDao = new ForMemoryGeneratedBenchmarkDao(new DapperExecutor(), new SingleConnectionManager(() => memoryCon));
            memoryGeneratedDao2 = new ForMemoryGeneratedBenchmarkDao2(new DapperExecutor(), new SingleConnectionManager(() => memoryCon));
        }

        private static void PrepareDatabase(IDbConnection con)
        {
            con.Execute("CREATE TABLE Data(Id INTEGER NOT NULL, Name TEXT NOT NULL, PRIMARY KEY(Id))");
            using (var tx = con.BeginTransaction())
            {
                for (var i = 0; i < 10000; i++)
                {
                    con.Execute("INSERT INTO Data(Id, Name) VALUES (@Id, @Name)", new { Id = i, Name = $"Name-{i}" });
                }

                tx.Commit();
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            memoryCon.Close();
        }

        [Benchmark]
        public void FileManual()
        {
            manualDao.QueryFirstOrDefault(1000);
        }

        [Benchmark]
        public void FileGenerated()
        {
            generatedDao.QueryFirstOrDefault(1000);
        }

        [Benchmark]
        public void FileGenerated2()
        {
            generatedDao2.QueryFirstOrDefault(1000);
        }

        [Benchmark]
        public void MemoryManual()
        {
            memoryManualDao.QueryFirstOrDefault(1000);
        }

        [Benchmark]
        public void MemoryGenerated()
        {
            memoryGeneratedDao.QueryFirstOrDefault(1000);
        }

        [Benchmark]
        public void MemoryGenerated2()
        {
            memoryGeneratedDao2.QueryFirstOrDefault(1000);
        }
    }

    public class DataEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public interface IBenchmarkDao
    {
        DataEntity QueryFirstOrDefault(int? id);
    }

    public sealed class ManualBenchmarkDao : IBenchmarkDao
    {
        private readonly Func<IDbConnection> factory;

        public ManualBenchmarkDao(Func<IDbConnection> factory)
        {
            this.factory = factory;
        }

        public DataEntity QueryFirstOrDefault(int? id)
        {
            var sql = new StringBuilder(32);
            sql.Append("SELECT * FROM Data");
            if (id != null)
            {
                sql.Append(" WHERE id = @id");
            }

            using (var con = factory())
            {
                return con.QueryFirstOrDefault<DataEntity>(sql.ToString(), new { id });
            }
        }
    }

    public sealed class ForMemoryManualBenchmarkDao : IBenchmarkDao
    {
        private readonly Func<IDbConnection> factory;

        public ForMemoryManualBenchmarkDao(Func<IDbConnection> factory)
        {
            this.factory = factory;
        }

        public DataEntity QueryFirstOrDefault(int? id)
        {
            var sql = new StringBuilder(32);
            sql.Append("SELECT * FROM Data");
            if (id != null)
            {
                sql.Append(" WHERE id = @id");
            }

            var con = factory();
            return con.QueryFirstOrDefault<DataEntity>(sql.ToString(), new { id });
        }
    }

    public interface IConnectionManager
    {
        Func<IDbConnection> GetFactory(string name);
    }

    public sealed class SingleConnectionManager : IConnectionManager
    {
        private readonly Func<IDbConnection> func;

        public SingleConnectionManager(Func<IDbConnection> func)
        {
            this.func = func;
        }

        public Func<IDbConnection> GetFactory(string name)
        {
            return func;
        }
    }

    public interface IParameter
    {
        void Add(string name, object value);
    }

    public interface IExecutor
    {
        IParameter CreateParameter();

        T QueryFirstOrDefault<T>(IDbConnection con, string sql, object parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);

        T QueryFirstOrDefault<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null);
    }

    public sealed class DapperParameter : IParameter
    {
        public DynamicParameters Parameters { get; } = new DynamicParameters();

        public void Add(string name, object value)
        {
            Parameters.Add(name, value);
        }
    }

    public sealed class DapperExecutor : IExecutor
    {
        public IParameter CreateParameter()
        {
            return new DapperParameter();
        }

        public T QueryFirstOrDefault<T>(IDbConnection con, string sql, object parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.QueryFirstOrDefault<T>(sql, parameter, tx, timeout, commandType);
        }

        public T QueryFirstOrDefault<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.QueryFirstOrDefault<T>(sql, (parameter as DapperParameter)?.Parameters, tx, timeout, commandType);
        }
    }

    public sealed class GeneratedBenchmarkDao : IBenchmarkDao
    {
        private readonly IExecutor executor;

        private readonly Func<IDbConnection> factory;

        public GeneratedBenchmarkDao(IExecutor executor, IConnectionManager connectionManager)
        {
            this.executor = executor;
            factory = connectionManager.GetFactory(string.Empty);
        }

        public DataEntity QueryFirstOrDefault(int? id)
        {
            var sql = new StringBuilder(32);

            sql.Append("SELECT * FROM Data");
            if (id != null)
            {
                sql.Append(" WHERE id = @id");
            }

            using (var con = factory())
            {
                return executor.QueryFirstOrDefault<DataEntity>(con, sql.ToString(), new { id });
            }
        }
    }

    public sealed class ForMemoryGeneratedBenchmarkDao : IBenchmarkDao
    {
        private readonly IExecutor executor;

        private readonly Func<IDbConnection> factory;

        public ForMemoryGeneratedBenchmarkDao(IExecutor executor, IConnectionManager connectionManager)
        {
            this.executor = executor;
            factory = connectionManager.GetFactory(string.Empty);
        }

        public DataEntity QueryFirstOrDefault(int? id)
        {
            var sql = new StringBuilder(32);

            sql.Append("SELECT * FROM Data");
            if (id != null)
            {
                sql.Append(" WHERE id = @id");
            }

            var con = factory();
            return executor.QueryFirstOrDefault<DataEntity>(con, sql.ToString(), new { id });
        }
    }

    public sealed class GeneratedBenchmarkDao2 : IBenchmarkDao
    {
        private readonly IExecutor executor;

        private readonly Func<IDbConnection> factory;

        public GeneratedBenchmarkDao2(IExecutor executor, IConnectionManager connectionManager)
        {
            this.executor = executor;
            factory = connectionManager.GetFactory(string.Empty);
        }

        public DataEntity QueryFirstOrDefault(int? id)
        {
            var parameter = executor.CreateParameter();
            var sql = new StringBuilder(32);

            sql.Append("SELECT * FROM Data");
            if (id != null)
            {
                sql.Append(" WHERE id = @id");
                parameter.Add("id", id);
            }

            using (var con = factory())
            {
                return executor.QueryFirstOrDefault<DataEntity>(con, sql.ToString(), parameter);
            }
        }
    }

    public sealed class ForMemoryGeneratedBenchmarkDao2 : IBenchmarkDao
    {
        private readonly IExecutor executor;

        private readonly Func<IDbConnection> factory;

        public ForMemoryGeneratedBenchmarkDao2(IExecutor executor, IConnectionManager connectionManager)
        {
            this.executor = executor;
            factory = connectionManager.GetFactory(string.Empty);
        }

        public DataEntity QueryFirstOrDefault(int? id)
        {
            var parameter = executor.CreateParameter();
            var sql = new StringBuilder(32);

            sql.Append("SELECT * FROM Data");
            if (id != null)
            {
                sql.Append(" WHERE id = @id");
                parameter.Add("id", id);
            }

            var con = factory();
            return executor.QueryFirstOrDefault<DataEntity>(con, sql.ToString(), parameter);
        }
    }
}
