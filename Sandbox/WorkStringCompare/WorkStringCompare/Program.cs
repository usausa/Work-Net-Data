using System.Diagnostics;

namespace WorkStringCompare
{
    using System;
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
        private const int N = 1000;

        private readonly string valueS1 = new string('X', 2);
        private readonly string valueS2 = new string('X', 2);
        private readonly string valueM1 = new string('X', 4);
        private readonly string valueM2 = new string('X', 4);
        private readonly string valueL1 = new string('X', 8);
        private readonly string valueL2 = new string('X', 8);
        private readonly string valueX1 = new string('X', 16);
        private readonly string valueX2 = new string('X', 16);

        //--------------------------------------------------------------------------------
        // Equals
        //--------------------------------------------------------------------------------

        [Benchmark(OperationsPerInvoke = N)]
        public bool EqualsS()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = valueS1 == valueS2;
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool EqualsM()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = valueM1 == valueM2;
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool EqualsL()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = valueL1 == valueL2;
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool EqualsX()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = valueX1 == valueX2;
            }
            return ret;
        }

        //--------------------------------------------------------------------------------
        // Array
        //--------------------------------------------------------------------------------

        [Benchmark(OperationsPerInvoke = N)]
        public bool ArrayS()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = valueS1.AsSpan().SequenceEqual(valueS2.AsSpan());
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool ArrayM()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = valueM1.AsSpan().SequenceEqual(valueM2.AsSpan());
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool ArrayL()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = valueL1.AsSpan().SequenceEqual(valueL2.AsSpan());
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool ArrayX()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = valueX1.AsSpan().SequenceEqual(valueX2.AsSpan());
            }
            return ret;
        }

        //--------------------------------------------------------------------------------
        // Custom
        //--------------------------------------------------------------------------------

        [Benchmark(OperationsPerInvoke = N)]
        public bool CustomS()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = CustomStringCompare.Compare(valueS1, valueS2);
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool CustomM()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = CustomStringCompare.Compare(valueM1, valueM2);
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool CustomL()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = CustomStringCompare.Compare(valueL1, valueL2);
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool CustomX()
        {
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = CustomStringCompare.Compare(valueX1, valueX2);
            }
            return ret;
        }
    }

    public static class CustomStringCompare
    {
        public static unsafe bool Compare(string value1, string value2)
        {
            // NotNull & NotObjectEquals
            var length = value1.Length;
            if (length != value2.Length)
            {
                return false;
            }

            fixed (char* pValue1 = value1)
            fixed (char* pValue2 = value2)
            {
                var p1 = pValue1;
                var p2 = pValue2;
                var last = p1 + length;
                var lastMinus8 = last - 4;
                while (p1 < lastMinus8)
                {
                    if (*(ulong*)p1 != *(ulong*)p2)
                    {
                        return false;
                    }

                    p1 += 4;
                    p2 += 4;
                }

                while (p1 < last)
                {
                    if (*p1 != *p2)
                    {
                        return false;
                    }

                    p1++;
                    p2++;
                }
            }

            return true;
        }
    }
}
