namespace ReaderBenchmark.Mock
{
    using System;
    using System.Collections;
    using System.Data.Common;
    using System.Linq;

    using Smart.Mock.Data;

    public sealed class TestDataReader : DbDataReader, IRepeatDataReader
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
}
