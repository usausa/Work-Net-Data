namespace HashBenchmark
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
            Add(Job.LongRun);
        }
    }

    public static class Data
    {
        public static Type TargetType { get; } = typeof(Target);

        public static ColumnInfo[] FixedColumns { get; }

        public static ColumnInfo[] VariableColumns { get; }

        static Data()
        {
            FixedColumns = new[]
            {
                new ColumnInfo("Id", typeof(long)),
                new ColumnInfo("Name", typeof(string)),
                new ColumnInfo("Amount", typeof(int)),
                new ColumnInfo("Flag", typeof(bool)),
                new ColumnInfo("DateTime", typeof(DateTime)),
            };

            VariableColumns = new ColumnInfo[FixedColumns.Length + 1];
            Array.Copy(FixedColumns, VariableColumns, FixedColumns.Length);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        [Benchmark]
        public int BasicArrayFixed() => HashMethods.BasicArrayFixed(Data.TargetType, Data.FixedColumns);

        [Benchmark]
        public int BasicArrayWithMax() => HashMethods.BasicArrayWithMax(Data.TargetType, Data.VariableColumns, Data.FixedColumns.Length);

        [Benchmark]
        public int BasicSpan() => HashMethods.BasicSpan(Data.TargetType, new Span<ColumnInfo>(Data.VariableColumns, 0, Data.FixedColumns.Length));

        [Benchmark]
        public int ByEach() => HashMethods.ByEach(Data.TargetType, Data.FixedColumns);

        [Benchmark]
        public int ByEach2() => HashMethods.ByEach2(Data.TargetType, Data.FixedColumns);

        [Benchmark]
        public int ByType2() => HashMethods.ByType2(Data.TargetType, Data.FixedColumns);
    }

    public static class HashMethods
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BasicArrayFixed(Type targetType, ColumnInfo[] columns)
        {
            unchecked
            {
                var hash = targetType.GetHashCode();
                for (var i = 0; i < columns.Length; i++)
                {
                    hash = (hash * 31) + columns[i].GetHashCode();
                }
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BasicArrayWithMax(Type targetType, ColumnInfo[] columns, int max)
        {
            unchecked
            {
                var hash = targetType.GetHashCode();
                for (var i = 0; i < (max <= columns.Length ? max : columns.Length); i++)
                {
                    hash = (hash * 31) + columns[i].GetHashCode();
                }
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BasicSpan(Type targetType, Span<ColumnInfo> columns)
        {
            unchecked
            {
                var hash = targetType.GetHashCode();
                for (var i = 0; i < columns.Length; i++)
                {
                    hash = (hash * 31) + columns[i].GetHashCode();
                }
                return hash;
            }
        }

        // Self

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ByEach(Type targetType, ColumnInfo[] columns)
        {
            unchecked
            {
                var hash = targetType.GetHashCode();
                for (var i = 0; i < columns.Length; i++)
                {
                    var column = columns[i];
                    hash = (hash * 31) + (column.Name.GetHashCode() ^ column.Type.GetHashCode());
                }
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ByEach2(Type targetType, ColumnInfo[] columns)
        {
            unchecked
            {
                var hash = targetType.GetHashCode();
                for (var i = 0; i < columns.Length; i++)
                {
                    hash = (hash * 31) + (columns[i].Name.GetHashCode() ^ columns[i].Type.GetHashCode());
                }
                return hash;
            }
        }

        // Type

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ByType2(Type targetType, ColumnInfo[] columns)
        {
            unchecked
            {
                var hash = targetType.GetHashCode();
                for (var i = 0; i < columns.Length; i++)
                {
                    hash = ((hash << 5) + hash) ^ (columns[i].Name.GetHashCode() ^ columns[i].Type.GetHashCode());
                }
                return hash;
            }
        }

        // TODO hash other
    }

    public struct ColumnInfo
    {
        public string Name { get; }

        public Type Type { get; }

        public ColumnInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public override int GetHashCode() => (Name, Type).GetHashCode();
    }

    public class Target
    {
    }
}
