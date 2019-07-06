namespace CommandPrepareBenchmark
{
    using System;
    using System.Data;
    using System.Text;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    using Smart.Mock.Data;

    public static class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<DataMapperBenchmark>();
        }
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(MarkdownExporter.Default, MarkdownExporter.GitHub);
            Add(MemoryDiagnoser.Default);
            Add(Job.LongRun);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
        "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Ignore")]
    [Config(typeof(BenchmarkConfig))]
    public class DataMapperBenchmark
    {
        private MockDbConnection con;

        [IterationSetup]
        public void IterationSetup()
        {
            con = new MockDbConnection();
            con.SetupCommand(cmd => cmd.SetupResult(1));
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            con.Dispose();
        }

        [Benchmark]
        public int BestOptimized() => Dao.OptimizedCode(con, null, null, null);

        [Benchmark]
        public int BestConditional() => Dao.ConditionalCode(con, null, null, null);

        [Benchmark]
        public int BestPrepared() => Dao.PreparedCode(con, null, null, null);

        [Benchmark]
        public int WorstOptimized() => Dao.OptimizedCode(con, "a", "b", "d");

        [Benchmark]
        public int WorstConditional() => Dao.ConditionalCode(con, "a", "b", "d");

        [Benchmark]
        public int WorstPrepared() => Dao.PreparedCode(con, "a", "b", "d");
    }

    public static class Dao
    {
        public static int OptimizedCode(MockDbConnection con, string value1, string value2, string value3)
        {
            using (var cmd = con.CreateCommand())
            {
                var sql = new StringBuilder(64);
                sql.Append("**********");

                if (!String.IsNullOrEmpty(value1))
                {
                    var param1 = cmd.CreateParameter();
                    param1.Value = value1;
                    param1.DbType = DbType.String;
                    cmd.Parameters.Add(param1);
                    sql.Append("**********");
                }

                if (!String.IsNullOrEmpty(value2))
                {
                    var param2 = cmd.CreateParameter();
                    param2.Value = value2;
                    param2.DbType = DbType.String;
                    cmd.Parameters.Add(param2);
                    sql.Append("**********");
                }

                if (!String.IsNullOrEmpty(value3))
                {
                    var param3 = cmd.CreateParameter();
                    param3.Value = value3;
                    param3.DbType = DbType.String;
                    cmd.Parameters.Add(param3);
                    sql.Append("**********");
                }

                cmd.CommandText = sql.ToString();

                con.Open();

                var result = cmd.ExecuteNonQuery();

                con.Close();

                return result;
            }
        }

        public static int ConditionalCode(MockDbConnection con, string value1, string value2, string value3)
        {
            using (var cmd = con.CreateCommand())
            {
                var flag1 = false;
                var flag2 = false;
                var flag3 = false;

                var sql = new StringBuilder(64);
                sql.Append("**********");

                if (!String.IsNullOrEmpty(value1))
                {
                    flag1 = true;
                    sql.Append("**********");
                }

                if (!String.IsNullOrEmpty(value2))
                {
                    flag2 = true;
                    sql.Append("**********");
                }

                if (!String.IsNullOrEmpty(value3))
                {
                    flag3 = true;
                    sql.Append("**********");
                }

                if (flag1)
                {
                    var param1 = cmd.CreateParameter();
                    param1.Value = value1;
                    param1.DbType = DbType.String;
                    cmd.Parameters.Add(param1);
                }

                if (flag2)
                {
                    var param2 = cmd.CreateParameter();
                    param2.Value = value2;
                    param2.DbType = DbType.String;
                    cmd.Parameters.Add(param2);
                }

                if (flag3)
                {
                    var param3 = cmd.CreateParameter();
                    param3.Value = value3;
                    param3.DbType = DbType.String;
                    cmd.Parameters.Add(param3);
                }

                cmd.CommandText = sql.ToString();

                con.Open();

                var result = cmd.ExecuteNonQuery();

                con.Close();

                return result;
            }
        }

        public static int PreparedCode(MockDbConnection con, string value1, string value2, string value3)
        {
            using (var cmd = con.CreateCommand())
            {
                var param1 = cmd.CreateParameter();
                param1.Value = value1;
                param1.DbType = DbType.String;
                cmd.Parameters.Add(param1);

                var param2 = cmd.CreateParameter();
                param2.Value = value2;
                param2.DbType = DbType.String;
                cmd.Parameters.Add(param2);

                var param3 = cmd.CreateParameter();
                param3.Value = value3;
                param3.DbType = DbType.String;
                cmd.Parameters.Add(param3);

                var sql = new StringBuilder(64);
                sql.Append("**********");

                if (!String.IsNullOrEmpty(value1))
                {
                    sql.Append("**********");
                }

                if (!String.IsNullOrEmpty(value2))
                {
                    sql.Append("**********");
                }

                if (!String.IsNullOrEmpty(value3))
                {
                    sql.Append("**********");
                }

                cmd.CommandText = sql.ToString();

                con.Open();

                var result = cmd.ExecuteNonQuery();

                con.Close();

                return result;
            }
        }
    }
}
