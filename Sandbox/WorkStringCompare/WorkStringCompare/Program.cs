namespace WorkStringCompare
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Intrinsics.X86;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Columns;
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
            AddExporter(MarkdownExporter.Default, MarkdownExporter.GitHub);
            AddColumn(
                StatisticColumn.Mean,
                StatisticColumn.Min,
                StatisticColumn.Max,
                StatisticColumn.P90,
                StatisticColumn.Error,
                StatisticColumn.StdDev);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.MediumRun);
            //AddJob(Job.ShortRun);
        }
    }

    public class Strings
    {
        public string Value1 { get; }

        public string Value2 { get; }

        public Strings(char c, int length)
        {
            Value1 = new string(c, length);
            Value2 = new string(c, length);
        }

        public override string ToString() => $"Length={Value1.Length:D2}";
    }

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private const int N = 1000;

        [ParamsSource(nameof(Values))]
        public Strings Value { get; set; }


        public IEnumerable<Strings> Values()
        {
            foreach (var length in new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 20 })
            {
                yield return new Strings('X', length);
            }
        }

        [Benchmark(OperationsPerInvoke = N, Baseline = true)]
        public bool Equals()
        {
            var value1 = Value.Value1;
            var value2 = Value.Value2;
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = value1 == value2;

            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool Span()
        {
            var value1 = Value.Value1;
            var value2 = Value.Value2;
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = value1.AsSpan().SequenceEqual(value2.AsSpan());
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool Native()
        {
            var value1 = Value.Value1;
            var value2 = Value.Value2;
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = CustomStringCompare.Native(value1, value2);
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool Unsafe()
        {
            var value1 = Value.Value1;
            var value2 = Value.Value2;
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = CustomStringCompare.Unsafe(value1, value2);
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool UnsafeBlock()
        {
            var value1 = Value.Value1;
            var value2 = Value.Value2;
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = CustomStringCompare.UnsafeBlock(value1, value2);
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool UnsafeBlock2()
        {
            var value1 = Value.Value1;
            var value2 = Value.Value2;
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = CustomStringCompare.UnsafeBlock2(value1, value2);
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool UnsafeBlock3()
        {
            var value1 = Value.Value1;
            var value2 = Value.Value2;
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = CustomStringCompare.UnsafeBlock3(value1, value2);
            }
            return ret;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool Intrinsics()
        {
            var value1 = Value.Value1;
            var value2 = Value.Value2;
            var ret = false;
            for (var i = 0; i < N; i++)
            {
                ret = CustomStringCompare.Intrinsics(value1, value2);
            }
            return ret;
        }
    }

    public static class CustomStringCompare
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Native(string value1, string value2)
        {
            // NotNull & NotObjectEquals
            if (value1.Length != value2.Length)
            {
                return false;
            }

            for (var i = 0; i < value1.Length; i++)
            {
                if (value1[i] != value2[i])
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Unsafe(string value1, string value2)
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
                for (var i = 0; i < length; i++)
                {
                    if (p1[i] != p2[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool UnsafeBlock(string value1, string value2)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool UnsafeBlock2(string value1, string value2)
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
                var i = 0;
                for (; i <= length - 4; i += 4)
                {
                    if (*(long*)(p1 + i) != *(long*)(p2 + i))
                    {
                        return false;
                    }
                }

                for (; i < length; i++)
                {
                    if (p1[i] != p2[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool UnsafeBlock3(string value1, string value2)
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
                var i = 0;
                for (; i <= length - 4; i += 4)
                {
                    if (*(long*)(p1 + i) != *(long*)(p2 + i))
                    {
                        return false;
                    }
                }

                var left = length - i;
                if (left >= 2)
                {
                    if (*(int*)(p1 + i) != *(int*)(p2 + i))
                    {
                        return false;
                    }

                    if (left == 3)
                    {
                        i += 2;
                        return p1[i] == p2[i];
                    }

                    return true;
                }

                if (left == 1)
                {
                    return p1[i] == p2[i];
                }

                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Intrinsics(string value1, string value2)
        {
            // NotNull & NotObjectEquals
            var length = value1.Length;
            if (length != value2.Length)
            {
                return false;
            }

            const int vectorSize = 128 / 8;
            const int equalsMask = unchecked((int) (0b1111_1111_1111_1111_1111_1111_1111_1111));
            fixed (char* pValue1 = value1)
            fixed (char* pValue2 = value2)
            {
                var ptrA = (byte*)pValue1;
                var ptrB = (byte*)pValue2;

                var i = 0;
                var size = length << 1;
                for (; i <= size - vectorSize; i += vectorSize)
                {
                    var va = Sse2.LoadVector128(ptrA + i);
                    var vb = Sse2.LoadVector128(ptrB + i);
                    var areEqual = Sse2.CompareEqual(va, vb);
                    if (Sse2.MoveMask(areEqual) != equalsMask)
                    {
                        return false;
                    }
                }

                // Not suitable < 16
                for (; i < size; i++)
                {
                    if (ptrA[i] != ptrB[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
