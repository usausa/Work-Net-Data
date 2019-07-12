namespace GeneratorBenchmark
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    using Smart.Data;
    using Smart.Mock.Data;

    public static class Program
    {
        public static void Main()
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
            Add(Job.LongRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private IDao staticDao;

        private IDao dynamicDao;

        private MockRepeatDbConnection mockExecute;

        [GlobalSetup]
        public void Setup()
        {
            mockExecute = new MockRepeatDbConnection(1);

            var provider = new DelegateDbProvider(() => mockExecute);
            staticDao = new StaticDao(provider);
            dynamicDao = DaoGenerator.Create(provider);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            mockExecute.Dispose();
        }

        [Benchmark]
        public int StaticDao() => staticDao.Execute();

        [Benchmark]
        public int DynamicDao() => dynamicDao.Execute();

    }

    public interface IDao
    {
        int Execute();
    }

    public static class DaoGenerator
    {
        public static IDao Create(IDbProvider provider)
        {
            var syntax = CSharpSyntaxTree.ParseText(Source);

            var references = new HashSet<Assembly>();
            AddReference(references, typeof(IDao).Assembly);
            AddReference(references, typeof(IDbProvider).Assembly);

            var metadataReferences = references
                .Select(x => MetadataReference.CreateFromFile(x.Location))
                .ToArray();

            var assemblyName = Path.GetRandomFileName();

            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release);

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntax },
                metadataReferences,
                options);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);
                var bytes = ms.ToArray();
                var assembly = Assembly.Load(bytes);

                var implementType = assembly.GetType("GeneratorBenchmark.DynamicDao");
                return (IDao)Activator.CreateInstance(implementType, provider);
            }
        }

        private static void AddReference(HashSet<Assembly> assemblies, Assembly assembly)
        {
            if (assemblies.Contains(assembly))
            {
                return;
            }

            assemblies.Add(assembly);

            foreach (var assemblyName in assembly.GetReferencedAssemblies())
            {
                AddReference(assemblies, Assembly.Load(assemblyName));
            }
        }

        private static readonly string Source = @"namespace GeneratorBenchmark
{
    using Smart.Data;

    public sealed class DynamicDao : IDao
    {
        private readonly IDbProvider provider;

        public DynamicDao(IDbProvider provider)
        {
            this.provider = provider;
        }

        public int Execute()
        {
            using (var con = provider.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                var param1 = cmd.CreateParameter();
                cmd.Parameters.Add(param1);
                var param2 = cmd.CreateParameter();
                cmd.Parameters.Add(param2);

                return cmd.ExecuteNonQuery();
            }
        }
    }
}";
    }
}
