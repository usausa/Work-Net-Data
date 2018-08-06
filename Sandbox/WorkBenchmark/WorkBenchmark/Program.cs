﻿using System.Data;
using System.Text;
using Dapper;

namespace WorkBenchmark
{
    using System;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

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
        private const int N = 10000;

        [GlobalSetup]
        public void Setup()
        {
            // TODO
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            // TODO
        }

        [Benchmark(OperationsPerInvoke = N)]
        public void SimpleManual()
        {
            for (var i = 0; i < N; i++)
            {
                // TODO
            }
        }

        [Benchmark(OperationsPerInvoke = N)]
        public void SimpleGenerated()
        {
            for (var i = 0; i < N; i++)
            {
                // TODO
            }
        }
    }

    public class DataEntity
    {
        public int Id { get; set; }

        public string Text { get; set; }
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

        public T QueryFirstOrDefault<T>(IDbConnection con, string sql, IParameter parameter = null, IDbTransaction tx = null, int? timeout = null, CommandType? commandType = null)
        {
            return con.QueryFirstOrDefault<T>(sql, (parameter as DapperParameter)?.Parameters, tx, timeout, commandType);
        }
    }

    public sealed class GeneratedBenchmarkDao : IBenchmarkDao
    {
        public DataEntity QueryFirstOrDefault(int? id)
        {
            throw new NotImplementedException();
        }
    }
}