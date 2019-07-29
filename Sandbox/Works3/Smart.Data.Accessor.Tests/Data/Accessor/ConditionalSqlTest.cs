namespace Smart.Data.Accessor
{
    using System.Collections.Generic;

    using Smart.Data.Accessor.Attributes;
    using Smart.Mock;

    using Xunit;

    public class ConditionalSqlTest
    {
        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IDynamicDao
        {
            [Query]
            IList<DataEntity> QueryData(int? id);
        }

        [Fact]
        public void UseHelper()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" })
                .InsertData(new DataEntity { Id = 3, Name = "Data-3" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql(
                        "SELECT * FROM Data" +
                        "/*% if (IsNotNull(id)) { */" +
                        "WHERE Id >= /*@ id */0" +
                        "/*% } */")
                    .Build();
                var dao = generator.Create<IDynamicDao>();

                var list = dao.QueryData(null);

                Assert.Equal(3, list.Count);

                list = dao.QueryData(2);

                Assert.Equal(2, list.Count);
            }
        }

        //--------------------------------------------------------------------------------
        // Custom
        //--------------------------------------------------------------------------------

        [Fact]
        public void UseCustomHelper()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" })
                .InsertData(new DataEntity { Id = 3, Name = "Data-3" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql(
                        "/*!helper Smart.Mock.CustomScriptHelper */" +
                        "SELECT * FROM Data" +
                        "/*% if (HasValue(id)) { */" +
                        "WHERE Id >= /*@ id */0" +
                        "/*% } */")
                    .Build();
                var dao = generator.Create<IDynamicDao>();

                var list = dao.QueryData(null);

                Assert.Equal(3, list.Count);

                list = dao.QueryData(2);

                Assert.Equal(2, list.Count);
            }
        }

        [Fact]
        public void UseCustomUsing()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" })
                .InsertData(new DataEntity { Id = 3, Name = "Data-3" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql(
                        "/*!using Smart.Mock */" +
                        "SELECT * FROM Data" +
                        "/*% if (CustomScriptHelper.HasValue(id)) { */" +
                        "WHERE Id >= /*@ id */0" +
                        "/*% } */")
                    .Build();
                var dao = generator.Create<IDynamicDao>();

                var list = dao.QueryData(null);

                Assert.Equal(3, list.Count);

                list = dao.QueryData(2);

                Assert.Equal(2, list.Count);
            }
        }
    }
}
