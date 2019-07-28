namespace Smart.Data.Accessor.Mappers
{
    using System;
    using System.Collections.Generic;

    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Engine;
    using Smart.Mock.Data;

    using Xunit;

    public class ObjectResultMapperFactoryTest
    {
        [Fact]
        public void MapProperty()
        {
            var engine = new ExecuteEngineConfig().ToEngine();

            var columns = new[]
            {
                new MockColumn(typeof(int), "Column1"),
                new MockColumn(typeof(int), "Column2"),
                new MockColumn(typeof(int), "Column3"),
                new MockColumn(typeof(int), "Column4"),
                new MockColumn(typeof(int), "Column5"),
                new MockColumn(typeof(int), "Column6"),
                new MockColumn(typeof(int), "Column7"),
                new MockColumn(typeof(int), "Column8"),
            };
            var values = new List<object[]>
            {
                new object[] { 1, 1, 1, 1, 1, 1, 1, 1 },
                new object[] { DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value }
            };

            var cmd = new MockDbCommand();
            cmd.SetupResult(new MockDataReader(columns, values));

            var list = engine.QueryBuffer<DataEntity>(cmd);

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0].Column1);
            Assert.Equal(1, list[0].Column2);
            Assert.Equal(1, list[0].Column3);
            Assert.Equal(Value.One, list[0].Column4);
            Assert.Equal(Value.One, list[0].Column5);
            Assert.Equal(0, list[0].Column6);
            Assert.Equal(0, list[0].Column7);
            Assert.Equal(0, list[1].Column1);
            Assert.Null(list[1].Column2);
            Assert.Equal(0, list[1].Column3);
            Assert.Equal(Value.Zero, list[1].Column4);
            Assert.Null(list[1].Column5);
            Assert.Equal(0, list[1].Column6);
            Assert.Equal(0, list[1].Column7);
        }

        public class DataEntity
        {
            public int Column1 { get; set; }

            public int? Column2 { get; set; }

            public long Column3 { get; set; }

            public Value Column4 { get; set; }

            public Value? Column5 { get; set; }

            public int Column6 => Column7;

            [Ignore]
            public int Column7 { get; set; }
        }

        public enum Value
        {
            Zero = 0,
            One = 1
        }

        [Fact]
        public void DefaultConstructorRequired()
        {
            var engine = new ExecuteEngineConfig().ToEngine();

            var columns = new[]
            {
                new MockColumn(typeof(int), "Id"),
            };
            var values = new List<object[]>
            {
                new object[] { 1 },
            };

            var cmd = new MockDbCommand();
            cmd.SetupResult(new MockDataReader(columns, values));

            Assert.Throws<ArgumentException>(() => engine.QueryBuffer<NoConstructor>(cmd));
        }

        public class NoConstructor
        {
            public int Id { get; set; }

            public NoConstructor(int id)
            {
                Id = id;
            }
        }
    }
}
