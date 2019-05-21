namespace WorkRuntimeBenchmark
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    using WorkRuntimeLibrary;

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
        private IFactory preCompiled;

        private IFactory runtimeCompiled;

        [GlobalSetup]
        public void Setup()
        {
            preCompiled = new PreCompiledFactory(new Engine());

            var generator = new Generator();
            runtimeCompiled = generator.Create<IFactory>();
        }

        [Benchmark]
        public object PreCompiled() => preCompiled.Create();

        [Benchmark]
        public object RuntimeCompiled() => runtimeCompiled.Create();
    }

    public interface IFactory
    {
        object Create();
    }

    public sealed class PreCompiledFactory : IFactory
    {
        private readonly Engine engine;

        public PreCompiledFactory(Engine engine)
        {
            this.engine = engine;
        }

        public object Create() => engine.Create();
    }
}
