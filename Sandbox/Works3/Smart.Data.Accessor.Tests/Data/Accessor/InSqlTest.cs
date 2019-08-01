namespace Smart.Data.Accessor
{
    using System;
    using System.Collections.Generic;

    using Smart.Data.Accessor.Attributes;
    using Smart.Mock;

    using Xunit;

    public class InSqlTest
    {
        //--------------------------------------------------------------------------------
        // Array
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IInArrayDao
        {
            [Query]
            IList<DataEntity> QueryData(int[] ids);
        }

        [Fact]
        public void InArray()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" })
                .InsertData(new DataEntity { Id = 3, Name = "Data-3" })
                .InsertData(new DataEntity { Id = 4, Name = "Data-4" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("SELECT * FROM Data WHERE Id IN /*@ ids */(2, 4)")
                    .Build();
                var dao = generator.Create<IInArrayDao>();

                var list = dao.QueryData(null);

                Assert.Equal(0, list.Count);

                list = dao.QueryData(Array.Empty<int>());

                Assert.Equal(0, list.Count);

                list = dao.QueryData(new[] { 2, 4 });

                Assert.Equal(2, list.Count);
            }
        }

        //--------------------------------------------------------------------------------
        // Array
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IInListDao
        {
            [Query]
            IList<DataEntity> QueryData(List<int> ids);
        }

        [Fact]
        public void InList()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" })
                .InsertData(new DataEntity { Id = 3, Name = "Data-3" })
                .InsertData(new DataEntity { Id = 4, Name = "Data-4" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("SELECT * FROM Data WHERE Id IN /*@ ids */(2, 4)")
                    .Build();
                var dao = generator.Create<IInListDao>();

                var list = dao.QueryData(null);

                Assert.Equal(0, list.Count);

                list = dao.QueryData(new List<int>());

                Assert.Equal(0, list.Count);

                list = dao.QueryData(new List<int> { 2, 4 });

                Assert.Equal(2, list.Count);
            }
        }
    }
}
