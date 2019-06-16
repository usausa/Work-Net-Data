using System.Collections;
using System.Collections.Generic;
namespace InBenchmark
{
    using System;
    using System.Linq;
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
            Add(Job.ShortRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private int[] intArray;
        private string[] stringArray;
        private List<int> intList;
        private List<string> stringList;

        [GlobalSetup]
        public void Setup()
        {
            intArray = Enumerable.Range(0, 10).ToArray();
            stringArray = Enumerable.Range(0, 10).Select(x => x.ToString()).ToArray();
            intList = Enumerable.Range(0, 10).ToList();
            stringList = Enumerable.Range(0, 10).Select(x => x.ToString()).ToList();
        }

        // ArrayRaw,  ILT, IET (, IL, IE no support)

        [Benchmark] public void IntArray() => Routine.ArrayProc(intArray);
        [Benchmark] public void StringArray() => Routine.ArrayProc(stringArray);
        //[Benchmark] public void IntArray2() => Routine.ArrayProc2(intArray);          // Too slow
        //[Benchmark] public void StringArray2() => Routine.ArrayProc2(stringArray);    // Too slow
        //[Benchmark] public void IntArray3() => Routine.ArrayProc3(intArray);
        //[Benchmark] public void StringArray3() => Routine.ArrayProc3(stringArray);    // ...
        [Benchmark] public void IntEnumerable() => Routine.EnumerableProc(intList);
        [Benchmark] public void StringEnumerable() => Routine.EnumerableProc(stringList);
        [Benchmark] public void IntList() => Routine.ListProc(intList);
        [Benchmark] public void StringList() => Routine.ListProc(stringList);
        [Benchmark] public void IntEnumerable2() => Routine.EnumerableProc2(intList);
        [Benchmark] public void StringEnumerable2() => Routine.EnumerableProc2(stringList);
        [Benchmark] public void IntList2() => Routine.ListProc2(intList);
        [Benchmark] public void StringList2() => Routine.ListProc2(stringList);
    }

    public static class Routine
    {
        private static void Call(object value)
        {
        }

        public static void ArrayProc<T>(T[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                Call(array[i]);
            }
        }

        public static void EnumerableProc<T>(IEnumerable<T> ie)
        {
            foreach (var value in ie)
            {
                Call(value);
            }
        }

        public static void ListProc<T>(IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                Call(list[i]);
            }
        }

        public static void ArrayProc2(Array array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                Call(array.GetValue(i));
            }
        }

        public static void EnumerableProc2(IEnumerable ie)
        {
            foreach (var value in ie)
            {
                Call(value);
            }
        }

        public static void ListProc2(IList list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                Call(list[i]);
            }
        }

        public static void ArrayProc3(object[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                Call(array[i]);
            }
        }
    }
}
