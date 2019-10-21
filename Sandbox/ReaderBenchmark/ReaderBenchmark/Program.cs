using System;
using System.Data;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Smart.Mock.Data;

namespace ReaderBenchmark
{
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
        private IDataReader reader;

        private Func<IDataRecord, Data> mapper1;
        private Func<IDataRecord, Data> mapper2;

        [GlobalSetup]
        public void Setup()
        {
            var properties = typeof(Data).GetProperties().ToArray();
            var columns = properties.Select(x => new ColumnInfo(x.Name, x.PropertyType)).ToArray();
            reader = CreateDataReader(columns.Length, x => properties[x].Name, x => $"Data-{x}");
            reader.Read();

            mapper1 = ObjectResultMapperFactory.Instance.CreateMapper<Data>(typeof(Data), columns);
            mapper2 = NewResultMapperFactory.Instance.CreateMapper<Data>(typeof(Data), columns);
        }

        private static MockDataReader CreateDataReader(int count, Func<int, string> naming, Func<int, object> valueFactory)
        {
            var columns = Enumerable.Range(0, count).Select(x => new MockColumn(typeof(long), naming(x))).ToArray();
            var values = Enumerable.Range(0, count).Select(valueFactory).ToArray();
            return new MockDataReader(columns, new[] { values });
        }

        [GlobalCleanup]
        public void Cleanup()
        {
        }

        [Benchmark]
        public Data Map1() => mapper1(reader);

        [Benchmark]
        public Data Map2() => mapper2(reader);
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
    }
}
