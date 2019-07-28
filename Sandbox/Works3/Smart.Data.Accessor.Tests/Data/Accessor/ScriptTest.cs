namespace Smart.Data.Accessor
{
    using System.Collections.Generic;

    using Smart.Data.Accessor.Attributes;
    using Smart.Mock;

    using Xunit;

    public class ScriptTest
    {
        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IScriptDao
        {
            [Query]
            IList<DataEntity> QueryData(int? id);
        }

        [Fact]
        public void QueryBufferdSimple()
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
                var dao = generator.Create<IScriptDao>();

                var list = dao.QueryData(null);

                Assert.Equal(3, list.Count);

                list = dao.QueryData(2);

                Assert.Equal(2, list.Count);
            }
        }
    }
}
