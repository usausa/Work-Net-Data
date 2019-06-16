using System.Data;
using System.Data.Common;
using Smart.Mock.Data;

namespace ParameterBuilderBenchmark
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
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

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private ITypeHandler handler;

        private Action<DbCommand, object> func1;

        private Action<DbCommand, object> func2;

        [GlobalSetup]
        public void Setup()
        {
            handler = new DummyTypeHandler();

            var builder1 = new ClassBuilderByDbType("p1", DbType.Object);
            func1 = builder1.Build;

            var builder2 = new ClassBuilderByTypeHandler("p1", handler);
            func2 = builder2.Build;
        }

        [Benchmark]
        public void FuncByDbType()
        {
            var cmd = new MockDbCommand();
            func1(cmd, string.Empty);
        }

        [Benchmark]
        public void FuncByHandler()
        {
            var cmd = new MockDbCommand();
            func2(cmd, string.Empty);
        }

        [Benchmark]
        public void CallByDbType()
        {
            var cmd = new MockDbCommand();
            ParameterBuilder.Build(cmd, "p1", DbType.Object, null, string.Empty);
        }

        [Benchmark]
        public void CallByDbHandler()
        {
            var cmd = new MockDbCommand();
            ParameterBuilder.Build(cmd, "p1", DbType.Object, handler, string.Empty);
        }
    }

    public interface ITypeHandler
    {
        void Build(DbParameter parameter, object value);
    }

    public sealed class DummyTypeHandler : ITypeHandler
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Build(DbParameter parameter, object value)
        {
            parameter.DbType = DbType.Object;
            parameter.Value = value;
        }
    }

    public sealed class ClassBuilderByDbType
    {
        private readonly string name;

        private readonly DbType dbType;

        public ClassBuilderByDbType(string name, DbType dbType)
        {
            this.name = name;
            this.dbType = dbType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Build(DbCommand cmd, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            if (value is null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                p.DbType = dbType;
                p.Value = value;
            }
        }
    }

    public sealed class ClassBuilderByTypeHandler
    {
        private readonly string name;

        private readonly ITypeHandler handler;

        public ClassBuilderByTypeHandler(string name, ITypeHandler handler)
        {
            this.name = name;
            this.handler = handler;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Build(DbCommand cmd, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            if (value is null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                handler.Build(p, value);
            }
        }
    }

    public static class ParameterBuilder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Build(DbCommand cmd, string name, DbType dbType, ITypeHandler handler, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            if (value is null)
            {
                p.Value = DBNull.Value;
            }
            else if (handler is null)
            {
                p.DbType = dbType;
                p.Value = value;
            }
            else
            {
                handler.Build(p, value);
            }
        }
    }
}
