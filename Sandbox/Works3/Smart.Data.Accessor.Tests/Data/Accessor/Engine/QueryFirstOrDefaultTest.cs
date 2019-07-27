namespace Smart.Data.Accessor.Engine
{
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;

    using Smart.Data.Accessor.Attributes;
    using Smart.Mock;

    using Xunit;

    public class QueryFirstOrDefaultTest
    {
        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IQueryFirstOrDefaultSimpleDao
        {
            [QueryFirstOrDefault]
            DataEntity QueryFirstOrDefault(long id);
        }

        [Fact]
        public void QueryFirstOrDefaultSimple()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("SELECT * FROM Data WHERE Id = /*@ id */1")
                    .Build();
                var dao = generator.Create<IQueryFirstOrDefaultSimpleDao>();

                var entity = dao.QueryFirstOrDefault(1L);

                Assert.NotNull(entity);
                Assert.Equal(1, entity.Id);
                Assert.Equal("Data-1", entity.Name);

                entity = dao.QueryFirstOrDefault(2L);
                Assert.Null(entity);
            }
        }

        [Dao]
        public interface IQueryFirstOrDefaultSimpleAsyncDao
        {
            [QueryFirstOrDefault]
            Task<DataEntity> QueryFirstOrDefaultAsync(long id);
        }

        [Fact]
        public async Task QueryFirstOrDefaultSimpleAsync()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("SELECT * FROM Data WHERE Id = /*@ id */1")
                    .Build();
                var dao = generator.Create<IQueryFirstOrDefaultSimpleAsyncDao>();

                var entity = await dao.QueryFirstOrDefaultAsync(1L);

                Assert.NotNull(entity);
                Assert.Equal(1, entity.Id);
                Assert.Equal("Data-1", entity.Name);

                entity = await dao.QueryFirstOrDefaultAsync(2L);
                Assert.Null(entity);
            }
        }

        //--------------------------------------------------------------------------------
        // With Connection
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IQueryFirstOrDefaultWithConnectionDao
        {
            [QueryFirstOrDefault]
            DataEntity QueryFirstOrDefault(DbConnection con, long id);
        }

        [Fact]
        public void QueryFirstOrDefaultWithConnection()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql("SELECT * FROM Data WHERE Id = /*@ id */1")
                    .Build();
                var dao = generator.Create<IQueryFirstOrDefaultWithConnectionDao>();

                con.Open();

                var entity = dao.QueryFirstOrDefault(con, 1L);

                Assert.Equal(ConnectionState.Open, con.State);
                Assert.NotNull(entity);
                Assert.Equal(1, entity.Id);
                Assert.Equal("Data-1", entity.Name);

                entity = dao.QueryFirstOrDefault(con, 2L);
                Assert.Null(entity);
            }
        }

        [Dao]
        public interface IQueryFirstOrDefaultWithConnectionAsyncDao
        {
            [QueryFirstOrDefault]
            Task<DataEntity> QueryFirstOrDefaultAsync(DbConnection con, long id);
        }

        [Fact]
        public async Task QueryFirstOrDefaultWithConnectionAsync()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql("SELECT * FROM Data WHERE Id = /*@ id */1")
                    .Build();
                var dao = generator.Create<IQueryFirstOrDefaultWithConnectionAsyncDao>();

                con.Open();

                var entity = await dao.QueryFirstOrDefaultAsync(con, 1L);

                Assert.Equal(ConnectionState.Open, con.State);
                Assert.NotNull(entity);
                Assert.Equal(1, entity.Id);
                Assert.Equal("Data-1", entity.Name);

                entity = await dao.QueryFirstOrDefaultAsync(con, 2L);
                Assert.Null(entity);
            }
        }

        // TODO
    }
}
