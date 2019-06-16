namespace FuncFactoryBenchmark
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
        private readonly MathEx math = new MathEx();

        private IMath mathIf;

        private Func<int, int, int> instanceMax;
        private Func<int, int, int> instanceMaxInline;
        private Func<int, int, int> classMax;
        private Func<int, int, int> classMaxInline;

        [GlobalSetup]
        public void Setup()
        {
            mathIf = math;
            instanceMax = math.InstanceMax;
            instanceMaxInline = math.InstanceMaxInline;
            classMax = MathEx.ClassMax;
            classMaxInline = MathEx.ClassMaxInline;
        }

        [Benchmark]
        public int InstanceMax() => math.InstanceMax(1, 0);
        [Benchmark]
        public int InstanceMaxInline() => math.InstanceMaxInline(1, 0);
        //[Benchmark]
        //public int ClassMax() => MathEx.ClassMax(1, 0);
        //[Benchmark]
        //public int ClassMaxInline() => MathEx.ClassMaxInline(1, 0);

        [Benchmark]
        public int InstanceMaxIf() => mathIf.InstanceMax(1, 0);
        [Benchmark]
        public int InstanceMaxInlineIf() => mathIf.InstanceMaxInline(1, 0);

        [Benchmark]
        public int InstanceMaxFunc() => instanceMax(1, 0);
        [Benchmark]
        public int InstanceMaxInlineFunc() => instanceMaxInline(1, 0);
        //[Benchmark]
        //public int ClassMaxFunc() => classMax(1, 0);
        //[Benchmark]
        //public int ClassMaxInlineFunc() => classMaxInline(1, 0);
    }

    public interface IMath
    {
        int InstanceMax(int x, int y);

        int InstanceMaxInline(int x, int y);
    }


    public sealed class MathEx : IMath
    {
        public int InstanceMax(int x, int y) => x > y ? x : y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int InstanceMaxInline(int x, int y) => x > y ? x : y;

        public static int ClassMax(int x, int y) => x > y ? x : y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClassMaxInline(int x, int y) => x > y ? x : y;
    }
}
