using System.Data;

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

        private readonly MyDaoConfig config = new MyDaoConfig();

        [GlobalSetup]
        public void Setup()
        {
            config.AddMap(typeof(IntEntity10), new Int10Mapper());
            config.AddMap(typeof(IntEntity20), new Int20Mapper());
            config.AddMap(typeof(StringEntity10), new String10Mapper());
            config.AddMap(typeof(StringEntity20), new String20Mapper());

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

        [Benchmark] public void BufferDapperInt100000() => Dapper.SqlMapper.Query<IntEntity10>(conInt100000, "", buffered: true);
        [Benchmark] public void BufferDapperInt101000() => Dapper.SqlMapper.Query<IntEntity10>(conInt101000, "", buffered: true);
        [Benchmark] public void BufferDapperInt102000() => Dapper.SqlMapper.Query<IntEntity10>(conInt102000, "", buffered: true);
        [Benchmark] public void BufferDapperInt200000() => Dapper.SqlMapper.Query<IntEntity20>(conInt200000, "", buffered: true);
        [Benchmark] public void BufferDapperInt201000() => Dapper.SqlMapper.Query<IntEntity20>(conInt201000, "", buffered: true);
        [Benchmark] public void BufferDapperInt202000() => Dapper.SqlMapper.Query<IntEntity20>(conInt202000, "", buffered: true);
        [Benchmark] public void BufferDapperString100000() => Dapper.SqlMapper.Query<StringEntity10>(conString100000, "", buffered: true);
        [Benchmark] public void BufferDapperString101000() => Dapper.SqlMapper.Query<StringEntity10>(conString101000, "", buffered: true);
        [Benchmark] public void BufferDapperString102000() => Dapper.SqlMapper.Query<StringEntity10>(conString102000, "", buffered: true);
        [Benchmark] public void BufferDapperString200000() => Dapper.SqlMapper.Query<StringEntity20>(conString200000, "", buffered: true);
        [Benchmark] public void BufferDapperString201000() => Dapper.SqlMapper.Query<StringEntity20>(conString201000, "", buffered: true);
        [Benchmark] public void BufferDapperString202000() => Dapper.SqlMapper.Query<StringEntity20>(conString202000, "", buffered: true);

        [Benchmark] public void NonBufferDapperInt100000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity10>(conInt100000, "", buffered: false)) { } }
        [Benchmark] public void NonBufferDapperInt101000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity10>(conInt101000, "", buffered: false)) { } }
        [Benchmark] public void NonBufferDapperInt102000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity10>(conInt102000, "", buffered: false)) { } }
        [Benchmark] public void NonBufferDapperInt200000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity20>(conInt200000, "", buffered: false)) { } }
        [Benchmark] public void NonBufferDapperInt201000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity20>(conInt201000, "", buffered: false)) { } }
        [Benchmark] public void NonBufferDapperInt202000() { foreach (var _ in Dapper.SqlMapper.Query<IntEntity20>(conInt202000, "", buffered: false)) { } }
        [Benchmark] public void NonBufferDapperString100000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity10>(conString100000, "", buffered: false)) { } }
        [Benchmark] public void NonBufferDapperString101000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity10>(conString101000, "", buffered: false)) { } }
        [Benchmark] public void NonBufferDapperString102000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity10>(conString102000, "", buffered: false)) { } }
        [Benchmark] public void NonBufferDapperString200000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity20>(conString200000, "", buffered: false)) { } }
        [Benchmark] public void NonBufferDapperString201000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity20>(conString201000, "", buffered: false)) { } }
        [Benchmark] public void NonBufferDapperString202000() { foreach (var _ in Dapper.SqlMapper.Query<StringEntity20>(conString202000, "", buffered: false)) { } }

        [Benchmark] public void BufferSmartInt100000() => conInt100000.QueryList<IntEntity10>("");
        [Benchmark] public void BufferSmartInt101000() => conInt101000.QueryList<IntEntity10>("");
        [Benchmark] public void BufferSmartInt102000() => conInt102000.QueryList<IntEntity10>("");
        [Benchmark] public void BufferSmartInt200000() => conInt200000.QueryList<IntEntity20>("");
        [Benchmark] public void BufferSmartInt201000() => conInt201000.QueryList<IntEntity20>("");
        [Benchmark] public void BufferSmartInt202000() => conInt202000.QueryList<IntEntity20>("");
        [Benchmark] public void BufferSmartString100000() => conString100000.QueryList<StringEntity10>("");
        [Benchmark] public void BufferSmartString101000() => conString101000.QueryList<StringEntity10>("");
        [Benchmark] public void BufferSmartString102000() => conString102000.QueryList<StringEntity10>("");
        [Benchmark] public void BufferSmartString200000() => conString200000.QueryList<StringEntity20>("");
        [Benchmark] public void BufferSmartString201000() => conString201000.QueryList<StringEntity20>("");
        [Benchmark] public void BufferSmartString202000() => conString202000.QueryList<StringEntity20>("");

        [Benchmark] public void BufferMyInt100000() => MyDao.Query<IntEntity10>(conInt100000, config, "");
        [Benchmark] public void BufferMyInt101000() => MyDao.Query<IntEntity10>(conInt101000, config, "");
        [Benchmark] public void BufferMyInt102000() => MyDao.Query<IntEntity10>(conInt102000, config, "");
        [Benchmark] public void BufferMyInt200000() => MyDao.Query<IntEntity20>(conInt200000, config, "");
        [Benchmark] public void BufferMyInt201000() => MyDao.Query<IntEntity20>(conInt201000, config, "");
        [Benchmark] public void BufferMyInt202000() => MyDao.Query<IntEntity20>(conInt202000, config, "");
        [Benchmark] public void BufferMyString100000() => MyDao.Query<StringEntity10>(conString100000, config, "");
        [Benchmark] public void BufferMyString101000() => MyDao.Query<StringEntity10>(conString101000, config, "");
        [Benchmark] public void BufferMyString102000() => MyDao.Query<StringEntity10>(conString102000, config, "");
        [Benchmark] public void BufferMyString200000() => MyDao.Query<StringEntity20>(conString200000, config, "");
        [Benchmark] public void BufferMyString201000() => MyDao.Query<StringEntity20>(conString201000, config, "");
        [Benchmark] public void BufferMyString202000() => MyDao.Query<StringEntity20>(conString202000, config, "");
    }

    public class MyDaoConfig
    {
        private readonly ThreadsafeTypeHashArrayMap<object> mappers = new ThreadsafeTypeHashArrayMap<object>();

        public void AddMap(Type type, object mapper)
        {
            mappers.AddIfNotExist(type, mapper);
        }

        public IMapper<T> GetMapper<T>()
        {
            return mappers.TryGetValue(typeof(T), out var mapper) ? (IMapper<T>)mapper : null;
        }

    }

    public static class MyDao
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

        public static List<T> Query<T>(DbConnection con, MyDaoConfig config, string sql, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var mapper = config.GetMapper<T>();

            con.Open();

            try
            {
                using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
                {
                    cmd.CommandText = sql;

                    using (var reader = cmd.ExecuteReader())
                    {
                        var list = new List<T>();
                        while (reader.Read())
                        {
                            list.Add(mapper.Map(reader));
                        }

                        return list;
                    }
                }
            }
            finally
            {
                con.Close();
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

    public interface IMapper<out T>
    {
        T Map(DbDataReader reader);
    }

    public sealed class Int10Mapper : IMapper<IntEntity10>
    {
        public IntEntity10 Map(DbDataReader reader)
        {
            var entity = new IntEntity10();
            entity.Id1 = Helper.GetValue<int>(reader, 0);
            entity.Id2 = Helper.GetValue<int>(reader, 1);
            entity.Id3 = Helper.GetValue<int>(reader, 2);
            entity.Id4 = Helper.GetValue<int>(reader, 3);
            entity.Id5 = Helper.GetValue<int>(reader, 4);
            entity.Id6 = Helper.GetValue<int>(reader, 5);
            entity.Id7 = Helper.GetValue<int>(reader, 6);
            entity.Id8 = Helper.GetValue<int>(reader, 7);
            entity.Id9 = Helper.GetValue<int>(reader, 8);
            entity.Id10 = Helper.GetValue<int>(reader, 9);
            return entity;
        }
    }

    public sealed class Int20Mapper : IMapper<IntEntity20>
    {
        public IntEntity20 Map(DbDataReader reader)
        {
            var entity = new IntEntity20();
            entity.Id1 = Helper.GetValue<int>(reader, 0);
            entity.Id2 = Helper.GetValue<int>(reader, 1);
            entity.Id3 = Helper.GetValue<int>(reader, 2);
            entity.Id4 = Helper.GetValue<int>(reader, 3);
            entity.Id5 = Helper.GetValue<int>(reader, 4);
            entity.Id6 = Helper.GetValue<int>(reader, 5);
            entity.Id7 = Helper.GetValue<int>(reader, 6);
            entity.Id8 = Helper.GetValue<int>(reader, 7);
            entity.Id9 = Helper.GetValue<int>(reader, 8);
            entity.Id10 = Helper.GetValue<int>(reader, 9);
            entity.Id11 = Helper.GetValue<int>(reader, 10);
            entity.Id12 = Helper.GetValue<int>(reader, 11);
            entity.Id13 = Helper.GetValue<int>(reader, 12);
            entity.Id14 = Helper.GetValue<int>(reader, 13);
            entity.Id15 = Helper.GetValue<int>(reader, 14);
            entity.Id16 = Helper.GetValue<int>(reader, 15);
            entity.Id17 = Helper.GetValue<int>(reader, 16);
            entity.Id18 = Helper.GetValue<int>(reader, 17);
            entity.Id19 = Helper.GetValue<int>(reader, 18);
            entity.Id20 = Helper.GetValue<int>(reader, 19);
            return entity;
        }
    }

    public sealed class String10Mapper : IMapper<StringEntity10>
    {
        public StringEntity10 Map(DbDataReader reader)
        {
            var entity = new StringEntity10();
            entity.Id1 = Helper.GetValue<string>(reader, 0);
            entity.Id2 = Helper.GetValue<string>(reader, 1);
            entity.Id3 = Helper.GetValue<string>(reader, 2);
            entity.Id4 = Helper.GetValue<string>(reader, 3);
            entity.Id5 = Helper.GetValue<string>(reader, 4);
            entity.Id6 = Helper.GetValue<string>(reader, 5);
            entity.Id7 = Helper.GetValue<string>(reader, 6);
            entity.Id8 = Helper.GetValue<string>(reader, 7);
            entity.Id9 = Helper.GetValue<string>(reader, 8);
            entity.Id10 = Helper.GetValue<string>(reader, 9);
            return entity;
        }
    }

    public sealed class String20Mapper : IMapper<StringEntity20>
    {
        public StringEntity20 Map(DbDataReader reader)
        {
            var entity = new StringEntity20();
            entity.Id1 = Helper.GetValue<string>(reader, 0);
            entity.Id2 = Helper.GetValue<string>(reader, 1);
            entity.Id3 = Helper.GetValue<string>(reader, 2);
            entity.Id4 = Helper.GetValue<string>(reader, 3);
            entity.Id5 = Helper.GetValue<string>(reader, 4);
            entity.Id6 = Helper.GetValue<string>(reader, 5);
            entity.Id7 = Helper.GetValue<string>(reader, 6);
            entity.Id8 = Helper.GetValue<string>(reader, 7);
            entity.Id9 = Helper.GetValue<string>(reader, 8);
            entity.Id10 = Helper.GetValue<string>(reader, 9);
            entity.Id11 = Helper.GetValue<string>(reader, 10);
            entity.Id12 = Helper.GetValue<string>(reader, 11);
            entity.Id13 = Helper.GetValue<string>(reader, 12);
            entity.Id14 = Helper.GetValue<string>(reader, 13);
            entity.Id15 = Helper.GetValue<string>(reader, 14);
            entity.Id16 = Helper.GetValue<string>(reader, 15);
            entity.Id17 = Helper.GetValue<string>(reader, 16);
            entity.Id18 = Helper.GetValue<string>(reader, 17);
            entity.Id19 = Helper.GetValue<string>(reader, 18);
            entity.Id20 = Helper.GetValue<string>(reader, 19);
            return entity;
        }
    }

    public class TestDataReader : DbDataReader, IRepeatDataReader
    {
        private readonly Type type;

        private readonly object value;

        private readonly string[] names;

        private readonly int records;

        private int current;

        public override int FieldCount => names.Length;

        public override int RecordsAffected => -1;

        public override bool HasRows => records > 0;

        public override bool IsClosed => false;

        public TestDataReader(Type type, object value, int records, int fields, string prefix)
        {
            this.type = type;
            this.value = value;
            names = Enumerable.Range(1, fields).Select(x => prefix + x).ToArray();
            this.records = records;

            current = -1;
        }

        public void Reset()
        {
            current = -1;
        }

        public override string GetName(int ordinal) => names[ordinal];

        public override Type GetFieldType(int ordinal) => type;

        public override int GetOrdinal(string name) => Array.IndexOf(names, name);


        public override object GetValue(int ordinal) => value;

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));

        public override bool NextResult() => current < records;

        public override bool Read()
        {
            current++;
            return current < records;
        }

        public override int Depth => 0;

        public override IEnumerator GetEnumerator() => new DbEnumerator(this, false);

        public override bool GetBoolean(int ordinal) => throw new NotSupportedException();
        public override byte GetByte(int ordinal) => throw new NotSupportedException();
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => throw new NotSupportedException();
        public override char GetChar(int ordinal) => throw new NotSupportedException();
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => throw new NotSupportedException();
        public override string GetDataTypeName(int ordinal) => throw new NotSupportedException();
        public override DateTime GetDateTime(int ordinal) => throw new NotSupportedException();
        public override decimal GetDecimal(int ordinal) => throw new NotSupportedException();
        public override double GetDouble(int ordinal) => throw new NotSupportedException();
        public override float GetFloat(int ordinal) => throw new NotSupportedException();
        public override Guid GetGuid(int ordinal) => throw new NotSupportedException();
        public override short GetInt16(int ordinal) => throw new NotSupportedException();
        public override int GetInt32(int ordinal) => throw new NotSupportedException();
        public override long GetInt64(int ordinal) => throw new NotSupportedException();
        public override string GetString(int ordinal) => throw new NotSupportedException();
        public override int GetValues(object[] values) => throw new NotSupportedException();

        public override bool IsDBNull(int ordinal) => throw new NotSupportedException();
    }

    public class IntEntity10
    {
        public int Id1 { get; set; }
        public int Id2 { get; set; }
        public int Id3 { get; set; }
        public int Id4 { get; set; }
        public int Id5 { get; set; }
        public int Id6 { get; set; }
        public int Id7 { get; set; }
        public int Id8 { get; set; }
        public int Id9 { get; set; }
        public int Id10 { get; set; }
    }

    public class IntEntity20
    {
        public int Id1 { get; set; }
        public int Id2 { get; set; }
        public int Id3 { get; set; }
        public int Id4 { get; set; }
        public int Id5 { get; set; }
        public int Id6 { get; set; }
        public int Id7 { get; set; }
        public int Id8 { get; set; }
        public int Id9 { get; set; }
        public int Id10 { get; set; }
        public int Id11 { get; set; }
        public int Id12 { get; set; }
        public int Id13 { get; set; }
        public int Id14 { get; set; }
        public int Id15 { get; set; }
        public int Id16 { get; set; }
        public int Id17 { get; set; }
        public int Id18 { get; set; }
        public int Id19 { get; set; }
        public int Id20 { get; set; }
    }

    public class StringEntity10
    {
        public string Id1 { get; set; }
        public string Id2 { get; set; }
        public string Id3 { get; set; }
        public string Id4 { get; set; }
        public string Id5 { get; set; }
        public string Id6 { get; set; }
        public string Id7 { get; set; }
        public string Id8 { get; set; }
        public string Id9 { get; set; }
        public string Id10 { get; set; }
    }

    public class StringEntity20
    {
        public string Id1 { get; set; }
        public string Id2 { get; set; }
        public string Id3 { get; set; }
        public string Id4 { get; set; }
        public string Id5 { get; set; }
        public string Id6 { get; set; }
        public string Id7 { get; set; }
        public string Id8 { get; set; }
        public string Id9 { get; set; }
        public string Id10 { get; set; }
        public string Id11 { get; set; }
        public string Id12 { get; set; }
        public string Id13 { get; set; }
        public string Id14 { get; set; }
        public string Id15 { get; set; }
        public string Id16 { get; set; }
        public string Id17 { get; set; }
        public string Id18 { get; set; }
        public string Id19 { get; set; }
        public string Id20 { get; set; }
    }
}
