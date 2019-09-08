namespace Smart.Data.Accessor.Builders
{
    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Attributes.Builders;
    using Smart.Data.Mapper;
    using Smart.Mock;

    using Xunit;

    public class InsertTest
    {
        //--------------------------------------------------------------------------------
        // Default
        //--------------------------------------------------------------------------------

        [DataAccessor]
        public interface IInsertDao
        {
            [Insert]
            int Insert(DataEntity entity);
        }

        [Fact]
        public void TestInsert()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable())
            {
                var generator = new TestFactoryBuilder()
                    .UseFileDatabase()
                    .Build();
                var dao = generator.Create<IInsertDao>();

                var effect = dao.Insert(new DataEntity { Id = 1, Name = "xxx" });

                Assert.Equal(1, effect);

                var entity = con.QueryData(1);
                Assert.NotNull(entity);
                Assert.Equal(1, entity.Id);
                Assert.Equal("xxx", entity.Name);
            }
        }

        //--------------------------------------------------------------------------------
        // DbValue
        //--------------------------------------------------------------------------------

        public class DbValueEntity
        {
            public long Id { get; set; }

            [DbValue("CURRENT_TIMESTAMP")]
            public string DateTime { get; set; }
        }

        [DataAccessor]
        public interface IInsertDbValueDao
        {
            [Insert]
            void Insert(DbValueEntity entity);

            [SelectSingle]
            DbValueEntity QueryEntity(long id);
        }

        [Fact]
        public void TestInsertDbValue()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable())
            {
                con.Execute("CREATE TABLE IF NOT EXISTS DbValue (Id int PRIMARY KEY, DateTime text)");

                var generator = new TestFactoryBuilder()
                    .UseFileDatabase()
                    .Build();
                var dao = generator.Create<IInsertDbValueDao>();

                dao.Insert(new DbValueEntity { Id = 1 });

                var entity = dao.QueryEntity(1);

                Assert.NotNull(entity);
                Assert.NotEmpty(entity.DateTime);
            }
        }

        //--------------------------------------------------------------------------------
        // CodeValue
        //--------------------------------------------------------------------------------

        public class Counter
        {
            private long counter;

            public long Next() => ++counter;
        }

        public class CodeValueEntity
        {
            public string Key { get; set; }

            [CodeValue("counter.Next()")]
            public long Value { get; set; }
        }

        [DataAccessor]
        [Inject(typeof(Counter), "counter")]
        public interface IInsertCodeValueDao
        {
            [Insert]
            void Insert(CodeValueEntity entity);

            [SelectSingle]
            CodeValueEntity QueryEntity(string key);
        }

        [Fact]
        public void TestInsertCodeValue()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable())
            {
                con.Execute("CREATE TABLE IF NOT EXISTS CodeValue (Key text PRIMARY KEY, Value int)");

                var generator = new TestFactoryBuilder()
                    .UseFileDatabase()
                    .ConfigureComponents(c => c.Add(new Counter()))
                    .Build();
                var dao = generator.Create<IInsertCodeValueDao>();

                dao.Insert(new CodeValueEntity { Key = "A" });
                dao.Insert(new CodeValueEntity { Key = "B" });

                var entityA = dao.QueryEntity("A");
                var entityB = dao.QueryEntity("B");

                Assert.NotNull(entityA);
                Assert.Equal(1, entityA.Value);

                Assert.NotNull(entityB);
                Assert.Equal(2, entityB.Value);
            }
        }
    }
}
