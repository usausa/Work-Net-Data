﻿using System.Reflection;
using Smart;
using Smart.Data.Mapper.Attributes;
using Smart.Reflection;

namespace ReaderBenchmark
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    using ReaderBenchmark.Mock;

    using System.Data;
    using Smart.Mock.Data;
    using Smart.Data.Mapper;
    using Smart.Collections.Concurrent;

    public static class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<DataMapperBenchmark>();
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
    public class DataMapperBenchmark
    {
        private MockRepeatDbConnection conInt100000;
        private MockRepeatDbConnection conInt101000;
        private MockRepeatDbConnection conInt102000;
        private MockRepeatDbConnection conInt200000;
        private MockRepeatDbConnection conInt201000;
        private MockRepeatDbConnection conInt202000;
        private MockRepeatDbConnection conString100000;
        private MockRepeatDbConnection conString101000;
        private MockRepeatDbConnection conString102000;
        private MockRepeatDbConnection conString200000;
        private MockRepeatDbConnection conString201000;
        private MockRepeatDbConnection conString202000;

        private readonly TestMapperConfig config = new TestMapperConfig();
        private readonly TestMapperConfig config2 = new TestMapperConfig();

        [GlobalSetup]
        public void Setup()
        {
            config.AddMap(typeof(IntEntity10), (Func<DbDataReader, IntEntity10>)new Int10Mapper().Map);
            config.AddMap(typeof(IntEntity20), (Func<DbDataReader, IntEntity20>)new Int20Mapper().Map);
            config.AddMap(typeof(StringEntity10), (Func<DbDataReader, StringEntity10>)new String10Mapper().Map);
            config.AddMap(typeof(StringEntity20), (Func<DbDataReader, StringEntity20>)new String20Mapper().Map);

            config2.AddMap(typeof(IntEntity10), ObjectResultMapperFactory.CreateMapper<IntEntity10>());
            config2.AddMap(typeof(IntEntity20), ObjectResultMapperFactory.CreateMapper<IntEntity20>());
            config2.AddMap(typeof(StringEntity10), ObjectResultMapperFactory.CreateMapper<StringEntity10>());
            config2.AddMap(typeof(StringEntity20), ObjectResultMapperFactory.CreateMapper<StringEntity20>());

            conInt100000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), 1, 0, 10, "Id"));
            conInt101000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), 1, 1000, 10, "Id"));
            conInt102000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), 1, 2000, 10, "Id"));
            conInt200000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), 1, 0, 20, "Id"));
            conInt201000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), 1, 1000, 20, "Id"));
            conInt202000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), 1, 2000, 20, "Id"));
            conString100000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), "a", 0, 10, "Id"));
            conString101000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), "a", 1000, 10, "Id"));
            conString102000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), "a", 2000, 10, "Id"));
            conString200000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), "a", 0, 20, "Id"));
            conString201000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), "a", 1000, 20, "Id"));
            conString202000 = new MockRepeatDbConnection(new TestDataReader(typeof(int), "a", 2000, 20, "Id"));
        }

        //[Benchmark] public void BufferDapperInt100000() => Dapper.SqlMapper.Query<IntEntity10>(conInt100000, "", buffered: true);
        //[Benchmark] public void BufferDapperInt101000() => Dapper.SqlMapper.Query<IntEntity10>(conInt101000, "", buffered: true);
        //[Benchmark] public void BufferDapperInt102000() => Dapper.SqlMapper.Query<IntEntity10>(conInt102000, "", buffered: true);
        //[Benchmark] public void BufferDapperInt200000() => Dapper.SqlMapper.Query<IntEntity20>(conInt200000, "", buffered: true);
        //[Benchmark] public void BufferDapperInt201000() => Dapper.SqlMapper.Query<IntEntity20>(conInt201000, "", buffered: true);
        //[Benchmark] public void BufferDapperInt202000() => Dapper.SqlMapper.Query<IntEntity20>(conInt202000, "", buffered: true);
        //[Benchmark] public void BufferDapperString100000() => Dapper.SqlMapper.Query<StringEntity10>(conString100000, "", buffered: true);
        //[Benchmark] public void BufferDapperString101000() => Dapper.SqlMapper.Query<StringEntity10>(conString101000, "", buffered: true);
        //[Benchmark] public void BufferDapperString102000() => Dapper.SqlMapper.Query<StringEntity10>(conString102000, "", buffered: true);
        //[Benchmark] public void BufferDapperString200000() => Dapper.SqlMapper.Query<StringEntity20>(conString200000, "", buffered: true);
        //[Benchmark] public void BufferDapperString201000() => Dapper.SqlMapper.Query<StringEntity20>(conString201000, "", buffered: true);
        //[Benchmark] public void BufferDapperString202000() => Dapper.SqlMapper.Query<StringEntity20>(conString202000, "", buffered: true);

        //[Benchmark] public void NonBufferDapperInt100000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity10>(conInt100000, "", buffered: false)) { } }
        //[Benchmark] public void NonBufferDapperInt101000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity10>(conInt101000, "", buffered: false)) { } }
        //[Benchmark] public void NonBufferDapperInt102000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity10>(conInt102000, "", buffered: false)) { } }
        //[Benchmark] public void NonBufferDapperInt200000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity20>(conInt200000, "", buffered: false)) { } }
        //[Benchmark] public void NonBufferDapperInt201000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity20>(conInt201000, "", buffered: false)) { } }
        //[Benchmark] public void NonBufferDapperInt202000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity20>(conInt202000, "", buffered: false)) { } }
        //[Benchmark] public void NonBufferDapperString100000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity10>(conString100000, "", buffered: false)) { } }
        //[Benchmark] public void NonBufferDapperString101000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity10>(conString101000, "", buffered: false)) { } }
        //[Benchmark] public void NonBufferDapperString102000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity10>(conString102000, "", buffered: false)) { } }
        //[Benchmark] public void NonBufferDapperString200000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity20>(conString200000, "", buffered: false)) { } }
        //[Benchmark] public void NonBufferDapperString201000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity20>(conString201000, "", buffered: false)) { } }
        //[Benchmark] public void NonBufferDapperString202000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity20>(conString202000, "", buffered: false)) { } }

        //[Benchmark] public void BufferSmartInt100000() => conInt100000.QueryList<IntEntity10>("");
        //[Benchmark] public void BufferSmartInt101000() => conInt101000.QueryList<IntEntity10>("");
        //[Benchmark] public void BufferSmartInt102000() => conInt102000.QueryList<IntEntity10>("");
        //[Benchmark] public void BufferSmartInt200000() => conInt200000.QueryList<IntEntity20>("");
        //[Benchmark] public void BufferSmartInt201000() => conInt201000.QueryList<IntEntity20>("");
        //[Benchmark] public void BufferSmartInt202000() => conInt202000.QueryList<IntEntity20>("");
        //[Benchmark] public void BufferSmartString100000() => conString100000.QueryList<StringEntity10>("");
        //[Benchmark] public void BufferSmartString101000() => conString101000.QueryList<StringEntity10>("");
        //[Benchmark] public void BufferSmartString102000() => conString102000.QueryList<StringEntity10>("");
        //[Benchmark] public void BufferSmartString200000() => conString200000.QueryList<StringEntity20>("");
        //[Benchmark] public void BufferSmartString201000() => conString201000.QueryList<StringEntity20>("");
        //[Benchmark] public void BufferSmartString202000() => conString202000.QueryList<StringEntity20>("");

        //[Benchmark] public void BufferMyInt100000() => TestMapper.Query<IntEntity10>(conInt100000, config, "");
        //[Benchmark] public void BufferMyInt101000() => TestMapper.Query<IntEntity10>(conInt101000, config, "");
        [Benchmark] public void BufferMyInt102000() => TestMapper.Query<IntEntity10>(conInt102000, config, "");
        //[Benchmark] public void BufferMyInt200000() => TestMapper.Query<IntEntity20>(conInt200000, config, "");
        //[Benchmark] public void BufferMyInt201000() => TestMapper.Query<IntEntity20>(conInt201000, config, "");
        [Benchmark] public void BufferMyInt202000() => TestMapper.Query<IntEntity20>(conInt202000, config, "");
        //[Benchmark] public void BufferMyString100000() => TestMapper.Query<StringEntity10>(conString100000, config, "");
        //[Benchmark] public void BufferMyString101000() => TestMapper.Query<StringEntity10>(conString101000, config, "");
        //[Benchmark] public void BufferMyString102000() => TestMapper.Query<StringEntity10>(conString102000, config, "");
        //[Benchmark] public void BufferMyString200000() => TestMapper.Query<StringEntity20>(conString200000, config, "");
        //[Benchmark] public void BufferMyString201000() => TestMapper.Query<StringEntity20>(conString201000, config, "");
        //[Benchmark] public void BufferMyString202000() => TestMapper.Query<StringEntity20>(conString202000, config, "");

        //[Benchmark] public void BufferMy2Int100000() => TestMapper.Query<IntEntity10>(conInt100000, config2, "");
        //[Benchmark] public void BufferMy2Int101000() => TestMapper.Query<IntEntity10>(conInt101000, config2, "");
        [Benchmark] public void BufferMy2Int102000() => TestMapper.Query<IntEntity10>(conInt102000, config2, "");
        //[Benchmark] public void BufferMy2Int200000() => TestMapper.Query<IntEntity20>(conInt200000, config2, "");
        //[Benchmark] public void BufferMy2Int201000() => TestMapper.Query<IntEntity20>(conInt201000, config2, "");
        [Benchmark] public void BufferMy2Int202000() => TestMapper.Query<IntEntity20>(conInt202000, config2, "");
        //[Benchmark] public void BufferMy2String100000() => TestMapper.Query<StringEntity10>(conString100000, config2, "");
        //[Benchmark] public void BufferMy2String101000() => TestMapper.Query<StringEntity10>(conString101000, config2, "");
        //[Benchmark] public void BufferMy2String102000() => TestMapper.Query<StringEntity10>(conString102000, config2, "");
        //[Benchmark] public void BufferMy2String200000() => TestMapper.Query<StringEntity20>(conString200000, config2, "");
        //[Benchmark] public void BufferMy2String201000() => TestMapper.Query<StringEntity20>(conString201000, config2, "");
        //[Benchmark] public void BufferMy2String202000() => TestMapper.Query<StringEntity20>(conString202000, config2, "");
    }

    public class TestMapperConfig
    {
        private readonly ThreadsafeTypeHashArrayMap<object> mappers = new ThreadsafeTypeHashArrayMap<object>();

        public void AddMap(Type type, object mapper)
        {
            mappers.AddIfNotExist(type, mapper);
        }

        public Func<DbDataReader, T> GetMapper<T>()
        {
            return mappers.TryGetValue(typeof(T), out var mapper) ? (Func<DbDataReader, T>)mapper : null;
        }
    }

    public static class TestMapper
    {
        private static DbCommand SetupCommand(DbConnection con, DbTransaction transaction, string sql, int? commandTimeout, CommandType? commandType)
        {
            var cmd = con.CreateCommand();

            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            cmd.CommandText = sql;

            if (commandTimeout.HasValue)
            {
                cmd.CommandTimeout = commandTimeout.Value;
            }

            if (commandType.HasValue)
            {
                cmd.CommandType = commandType.Value;
            }

            return cmd;
        }

        public static List<T> Query<T>(DbConnection con, TestMapperConfig config, string sql, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var mapper = config.GetMapper<T>();

            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                // TODO pre

                if (wasClosed)
                {
                    con.Open();
                }

                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        // TODO post

                        var list = new List<T>();
                        while (reader.Read())
                        {
                            list.Add(mapper(reader));
                        }

                        return list;
                    }
                }
                finally
                {
                    if (wasClosed)
                    {
                        con.Close();
                    }
                }
            }
        }
    }

    public static class Helper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetValue<T>(DbDataReader reader, int index)
        {
            var value = reader.GetValue(index);
            return value is DBNull ? default : (T)value;
        }
    }

    public static class ObjectResultMapperFactory
    {
        public static Func<IDataRecord, T> CreateMapper<T>()
        {
            var objectFactory = DelegateFactory.Default.CreateFactory<T>();
            var entries = CreateMapEntries(typeof(T));

            return record =>
            {
                var obj = objectFactory();

                for (var i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    entry.Setter(obj, record.GetValue(i));
                }

                return obj;
            };
        }

        private static MapEntry[] CreateMapEntries(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(IsTargetProperty)
                .ToArray();

            var list = new List<MapEntry>();
            foreach (var pi in properties)
            {
                var setter = DelegateFactory.Default.CreateSetter(pi);
                var defaultValue = pi.PropertyType.GetDefaultValue();

                list.Add(new MapEntry((obj, value) => setter(obj, value is DBNull ? defaultValue : value)));
            }

            return list.ToArray();
        }

        private static bool IsTargetProperty(PropertyInfo pi)
        {
            return pi.CanWrite;
            //return pi.CanWrite && (pi.GetCustomAttribute<IgnoreAttribute>() == null);
        }

        private sealed class MapEntry
        {
            public Action<object, object> Setter { get; }

            public MapEntry(Action<object, object> setter)
            {
                Setter = setter;
            }
        }
    }
}
