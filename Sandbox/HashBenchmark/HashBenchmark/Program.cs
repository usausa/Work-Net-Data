namespace HashBenchmark
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

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
            Add(Job.MediumRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private HashEmulator emulator1;

        private IDataReader reader1;

        private HashEmulator emulator2;

        private IDataReader reader2;

        [GlobalSetup]
        public void Setup()
        {
            var properties = typeof(Data).GetProperties().ToArray();

            var columns1 = properties.Select(x => new ColumnInfo { Name = x.Name, Type = x.PropertyType }).ToArray();
            reader1 = CreateDataReader(
                columns1.Length,
                x => new MockColumn(properties[x].PropertyType, properties[x].Name),
                x => properties[x].PropertyType == typeof(string)
                    ? (object)"test"
                    : 1);
            reader1.Read();

            emulator1 = new HashEmulator();
            emulator1.UpdateColumns<Data>(columns1);

            var columns2 = properties.Select(x => new ColumnInfo { Name = x.Name, Type = x.PropertyType }).Take(4).ToArray();
            reader2 = CreateDataReader(
                columns2.Length,
                x => new MockColumn(properties[x].PropertyType, properties[x].Name),
                x =>
                {
                    if (((x >= 4) && (x <= 7)) || ((x >= 12) && (x <= 15)))
                    {
                        return DBNull.Value;
                    }

                    return properties[x].PropertyType == typeof(string)
                        ? (object)"test"
                        : 1;
                });
            reader2.Read();

            emulator2 = new HashEmulator();
            emulator2.UpdateColumns<Data>(columns2);
        }

        private static MockDataReader CreateDataReader(int count, Func<int, MockColumn> columnFactory, Func<int, object> valueFactory)
        {
            var columns = Enumerable.Range(0, count).Select(columnFactory).ToArray();
            var values = Enumerable.Range(0, count).Select(valueFactory).ToArray();
            return new MockDataReader(columns, new[] { values });
        }

        [Benchmark]
        public void BaseImplement1() => emulator1.BaseImplement<Data>(reader1);

        [Benchmark]
        public void BaseImplement2() => emulator2.BaseImplement<Data>(reader2);

        [Benchmark]
        public void OtherImplement1() => emulator1.OtherImplement<Data>(reader1);

        [Benchmark]
        public void OtherImplement2() => emulator2.OtherImplement<Data>(reader2);
    }

    public sealed class HashEmulator
    {
        private ColumnInfo[] columns;

        private int columnsHash1;
        private int columnsHash2;

        [ThreadStatic]
        private static ColumnInfo[] columnInfoPool;

        public void UpdateColumns<T>(ColumnInfo[] value)
        {
            columns = value;
            columnsHash1 = CalculateHash(typeof(T), value);
            columnsHash2 = CalculateHash2(typeof(T), value);
        }

        public void BaseImplement<T>(IDataRecord record)
        {
            // Prepare
            var fieldCount = record.FieldCount;
            if ((columnInfoPool == null) || (columnInfoPool.Length < fieldCount))
            {
                var start = columnInfoPool?.Length ?? 0;
                columnInfoPool = new ColumnInfo[fieldCount];
                for (var i = start; i < columnInfoPool.Length; i++)
                {
                    columnInfoPool[i] = new ColumnInfo();
                }
            }

            var type = typeof(T);
            for (var i = 0; i < record.FieldCount; i++)
            {
                var column = columnInfoPool[i];
                column.Name = record.GetName(i);
                column.Type = record.GetFieldType(i);
            }

            var cols = new Span<ColumnInfo>(columnInfoPool, 0, fieldCount);

            // Hash
            var hash = CalculateHash(type, cols);
            if (hash != columnsHash1)
            {
                throw new Exception("Hash error 1.");
            }

            // Match
            var ret = IsMatchColumn(columns, cols);
            if (!ret)
            {
                throw new Exception("Hash error 2.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CalculateHash(Type targetType, Span<ColumnInfo> columns)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsMatchColumn(ColumnInfo[] columns1, Span<ColumnInfo> columns2)
        {
            if (columns1.Length != columns2.Length)
            {
                return false;
            }

            for (var i = 0; i < columns1.Length; i++)
            {
                var column1 = columns1[i];
                var column2 = columns2[i];

                if (column1.Type != column2.Type)
                {
                    return false;
                }

                if (String.Compare(column1.Name, column2.Name, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void OtherImplement<T>(IDataRecord record)
        {
            // Prepare
            var fieldCount = record.FieldCount;
            if ((columnInfoPool == null) || (columnInfoPool.Length < fieldCount))
            {
                var start = columnInfoPool?.Length ?? 0;
                columnInfoPool = new ColumnInfo[fieldCount];
                for (var i = start; i < columnInfoPool.Length; i++)
                {
                    columnInfoPool[i] = new ColumnInfo();
                }
            }

            var type = typeof(T);
            for (var i = 0; i < record.FieldCount; i++)
            {
                var column = columnInfoPool[i];
                column.Name = record.GetName(i);
                column.Type = record.GetFieldType(i);
            }

            var cols = new Span<ColumnInfo>(columnInfoPool, 0, fieldCount);

            // Hash
            var hash = CalculateHash2(type, cols);
            if (hash != columnsHash2)
            {
                throw new Exception("Hash error 1.");
            }

            // Match
            var ret = IsMatchColumn2(columns, cols);
            if (!ret)
            {
                throw new Exception("Hash error 2.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CalculateHash2(Type targetType, Span<ColumnInfo> columns)
        {
            unchecked
            {
                var hash = targetType.GetHashCode();
                for (var i = 0; i < columns.Length; i++)
                {
                    var column = columns[i];
                    hash ^= column.Name.GetHashCode();
                    hash ^= column.Type.GetHashCode();
                }
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsMatchColumn2(ColumnInfo[] columns1, Span<ColumnInfo> columns2)
        {
            if (columns1.Length != columns2.Length)
            {
                return false;
            }

            for (var i = 0; i < columns1.Length; i++)
            {
                var column1 = columns1[i];
                var column2 = columns2[i];

                if (column1.Type != column2.Type)
                {
                    return false;
                }

                if (String.Compare(column1.Name, column2.Name, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class Data
    {
        public string Name01 { get; set; }
        public string Name02 { get; set; }
        public string Name03 { get; set; }
        public string Name04 { get; set; }
        public string Name05 { get; set; }
        public string Name06 { get; set; }
        public string Name07 { get; set; }
        public string Name08 { get; set; }
        public string Name09 { get; set; }
        public string Name10 { get; set; }
        public string Name11 { get; set; }
        public string Name12 { get; set; }
        public string Name13 { get; set; }
        public string Name14 { get; set; }
        public string Name15 { get; set; }
        public string Name16 { get; set; }
        public int Value01 { get; set; }
        public int Value02 { get; set; }
        public int Value03 { get; set; }
        public int Value04 { get; set; }
        public int Value05 { get; set; }
        public int Value06 { get; set; }
        public int Value07 { get; set; }
        public int Value08 { get; set; }
        public int Value09 { get; set; }
        public int Value10 { get; set; }
        public int Value11 { get; set; }
        public int Value12 { get; set; }
        public int Value13 { get; set; }
        public int Value14 { get; set; }
        public int Value15 { get; set; }
        public int Value16 { get; set; }
    }

    public class ColumnInfo
    {
        public string Name { get; set; }

        public Type Type { get; set; }
    }
}
