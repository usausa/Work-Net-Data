namespace TestBenchmark
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    using Smart.Data.Accessor.Engine;
    using Smart.Mock.Data;

    public static class Program
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
            Add(Job.MediumRun);
            //Add(Job.ShortRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private ExecuteEngine engine;

        private MethodResultMapperCache<SimpleDataEntity> simpleCache;

        private IDataReader simpleReader;

        [GlobalSetup]
        public void Setup()
        {
            engine = new ExecuteEngineConfig()
                .ToEngine();
            simpleCache = new MethodResultMapperCache<SimpleDataEntity>(engine);
            simpleReader = new MockDataReader(SimpleDataEntity.Columns, new List<object[]>());
        }

        [Benchmark]
        public Func<IDataRecord, SimpleDataEntity> OldSimple() =>
            engine.CreateResultMapper<SimpleDataEntity>(simpleReader);

        [Benchmark]
        public Func<IDataRecord, SimpleDataEntity> NewSimple() =>
            simpleCache.ResolveMapper(simpleReader);
    }

    public class SimpleDataEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public static MockColumn[] Columns { get; } =
        {
            new MockColumn(typeof(long), nameof(Id)),
            new MockColumn(typeof(string), nameof(Name))
        };
    }
}
