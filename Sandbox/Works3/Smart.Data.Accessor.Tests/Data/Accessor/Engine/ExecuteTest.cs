namespace Smart.Data.Accessor.Engine
{
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;

    using Smart.Data.Accessor.Attributes;
    using Smart.Mock;

    using Xunit;

    public class ExecuteTest
    {
        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IExecuteSimpleDao
        {
            [Execute]
            int Execute(long id, string name);
        }

        [Fact]
        public void ExecuteSimple()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable())
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("INSERT INTO Data (Id, Name) VALUES (/*@ id */1, /*@ name */'test')")
                    .Build();
                var dao = generator.Create<IExecuteSimpleDao>();

                var effect = dao.Execute(2, "xxx");

                Assert.Equal(1, effect);

                var entity = con.QueryData(2);
                Assert.NotNull(entity);
                Assert.Equal(2, entity.Id);
                Assert.Equal("xxx", entity.Name);
            }
        }

        [Dao]
        public interface IExecuteSimpleAsyncDao
        {
            [Execute]
            Task<int> ExecuteAsync(long id, string name);
        }

        [Fact]
        public async Task ExecuteSimpleAsync()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable())
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("INSERT INTO Data (Id, Name) VALUES (/*@ id */1, /*@ name */'test')")
                    .Build();
                var dao = generator.Create<IExecuteSimpleAsyncDao>();

                var effect = await dao.ExecuteAsync(2, "xxx");

                Assert.Equal(1, effect);

                var entity = con.QueryData(2);
                Assert.NotNull(entity);
                Assert.Equal(2, entity.Id);
                Assert.Equal("xxx", entity.Name);
            }
        }

        //--------------------------------------------------------------------------------
        // With Connection
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IExecuteWithConnectionDao
        {
            [Execute]
            int Execute(DbConnection con, long id, string name);
        }

        [Fact]
        public void ExecuteWithConnection()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable())
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql("INSERT INTO Data (Id, Name) VALUES (/*@ id */1, /*@ name */'test')")
                    .Build();
                var dao = generator.Create<IExecuteWithConnectionDao>();

                con.Open();

                var effect = dao.Execute(con, 2, "xxx");

                Assert.Equal(ConnectionState.Open, con.State);
                Assert.Equal(1, effect);

                var entity = con.QueryData(2);
                Assert.NotNull(entity);
                Assert.Equal(2, entity.Id);
                Assert.Equal("xxx", entity.Name);
            }
        }

        [Dao]
        public interface IExecuteWithConnectionAsyncDao
        {
            [Execute]
            Task<int> ExecuteAsync(DbConnection con, long id, string name);
        }

        [Fact]
        public async Task ExecuteWithConnectionAsync()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable())
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql("INSERT INTO Data (Id, Name) VALUES (/*@ id */1, /*@ name */'test')")
                    .Build();
                var dao = generator.Create<IExecuteWithConnectionAsyncDao>();

                con.Open();

                var effect = await dao.ExecuteAsync(con, 2, "xxx");

                Assert.Equal(ConnectionState.Open, con.State);
                Assert.Equal(1, effect);

                var entity = con.QueryData(2);
                Assert.NotNull(entity);
                Assert.Equal(2, entity.Id);
                Assert.Equal("xxx", entity.Name);
            }
        }

        // TODO
    }
}
