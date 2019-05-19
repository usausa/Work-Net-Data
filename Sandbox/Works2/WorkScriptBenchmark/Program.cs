using WorkLibrary;

namespace WorkScriptBenchmark
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

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
        private IExecutor preCompiled;

        //private IExecutor runtimeCompiled;

        [GlobalSetup]
        public void Setup()
        {
            preCompiled = new PreCompiledExecutor(new Engine());

            // TODO
        }

        [Benchmark]
        public object PreCompiled() => preCompiled.Create();

        //[Benchmark]
        //public object RuntimeCompiled() => runtimeCompiled.Create();
    }

    public sealed class PreCompiledExecutor : IExecutor
    {
        private readonly Engine engine;

        public PreCompiledExecutor(Engine engine)
        {
            this.engine = engine;
        }

        public object Create() => engine.Create();
    }
}
