namespace Smart.Data.Accessor.Builders
{
    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Attributes.Builders;
    using Smart.Mock;

    using Xunit;

    public class UpdateTest
    {
        //--------------------------------------------------------------------------------
        // Key
        //--------------------------------------------------------------------------------

        [DataAccessor]
        public interface IUpdateByKeyDao
        {
            [Update]
            int Update(MultiKeyEntity entity);
        }

        [Fact]
        public void TestUpdateByKey()
        {
            using (var con = TestDatabase.Initialize()
                .SetupMultiKeyTable()
                .InsertMultiKey(new MultiKeyEntity { Key1 = 1, Key2 = 2, Type = "A", Name = "Data-1" }))
            {
                var generator = new TestFactoryBuilder()
                    .UseFileDatabase()
                    .Build();
                var dao = generator.Create<IUpdateByKeyDao>();

                var effect = dao.Update(new MultiKeyEntity { Key1 = 1, Key2 = 2, Type = "B", Name = "Data-2" });

                Assert.Equal(1, effect);

                var entity = con.QueryMultiKey(1, 2);
                Assert.NotNull(entity);
                Assert.Equal("B", entity.Type);
                Assert.Equal("Data-2", entity.Name);
            }
        }

        //--------------------------------------------------------------------------------
        // Values
        //--------------------------------------------------------------------------------

        public class UpdateValues
        {
            public string Type { get; set; }

            public string Name { get; set; }
        }

        [DataAccessor]
        public interface IUpdateWithValuesDao
        {
            [Update(typeof(MultiKeyEntity))]
            int Update([Values] UpdateValues values, long key1, string type);
        }

        [Fact]
        public void TestUpdateWithValues()
        {
            using (var con = TestDatabase.Initialize()
                .SetupMultiKeyTable()
                .InsertMultiKey(new MultiKeyEntity { Key1 = 1, Key2 = 1, Type = "A", Name = "Data-1" })
                .InsertMultiKey(new MultiKeyEntity { Key1 = 1, Key2 = 2, Type = "B", Name = "Data-2" })
                .InsertMultiKey(new MultiKeyEntity { Key1 = 1, Key2 = 3, Type = "A", Name = "Data-3" }))
            {
                var generator = new TestFactoryBuilder()
                    .UseFileDatabase()
                    .Build();
                var dao = generator.Create<IUpdateWithValuesDao>();

                var effect = dao.Update(new UpdateValues { Type = "B", Name = "Xxx" }, 1, "A");

                Assert.Equal(2, effect);

                var entity = con.QueryMultiKey(1, 1);
                Assert.NotNull(entity);
                Assert.Equal("B", entity.Type);
                Assert.Equal("Xxx", entity.Name);

                entity = con.QueryMultiKey(1, 3);
                Assert.NotNull(entity);
                Assert.Equal("B", entity.Type);
                Assert.Equal("Xxx", entity.Name);
            }
        }

        //--------------------------------------------------------------------------------
        // Condition
        //--------------------------------------------------------------------------------

        [DataAccessor]
        public interface IUpdateByConditionDao
        {
            [Update(typeof(MultiKeyEntity))]
            int Update(string type, string name, [Condition] long key1, [Condition][Name("type")] string conditionType);
        }

        [Fact]
        public void TestUpdateByCondition()
        {
            using (var con = TestDatabase.Initialize()
                .SetupMultiKeyTable()
                .InsertMultiKey(new MultiKeyEntity { Key1 = 1, Key2 = 1, Type = "A", Name = "Data-1" })
                .InsertMultiKey(new MultiKeyEntity { Key1 = 1, Key2 = 2, Type = "B", Name = "Data-2" })
                .InsertMultiKey(new MultiKeyEntity { Key1 = 1, Key2 = 3, Type = "A", Name = "Data-3" }))
            {
                var generator = new TestFactoryBuilder()
                    .UseFileDatabase()
                    .Build();
                var dao = generator.Create<IUpdateByConditionDao>();

                var effect = dao.Update("B", "Xxx", 1, "A");

                Assert.Equal(2, effect);

                var entity = con.QueryMultiKey(1, 1);
                Assert.NotNull(entity);
                Assert.Equal("B", entity.Type);
                Assert.Equal("Xxx", entity.Name);

                entity = con.QueryMultiKey(1, 3);
                Assert.NotNull(entity);
                Assert.Equal("B", entity.Type);
                Assert.Equal("Xxx", entity.Name);
            }
        }
    }
}
