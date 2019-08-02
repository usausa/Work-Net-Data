namespace Smart.Data.Accessor.Engine
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Generator;
    using Smart.Mock;

    using Xunit;

    public class QueryTest
    {
        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IQuerySimpleDao
        {
            [Query]
            IList<DataEntity> QueryBufferd();

            [Query]
            IEnumerable<DataEntity> QueryNonBufferd();
        }

        [Fact]
        public void QueryBufferdSimple()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("SELECT * FROM Data ORDER BY Id")
                    .Build();
                var dao = generator.Create<IQuerySimpleDao>();

                var list = dao.QueryBufferd();

                Assert.Equal(2, list.Count);
                Assert.Equal(1, list[0].Id);
                Assert.Equal("Data-1", list[0].Name);
                Assert.Equal(2, list[1].Id);
                Assert.Equal("Data-2", list[1].Name);
            }
        }

        [Fact]
        public void QueryNonBufferdSimple()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("SELECT * FROM Data ORDER BY Id")
                    .Build();
                var dao = generator.Create<IQuerySimpleDao>();

                var list = dao.QueryNonBufferd().ToList();

                Assert.Equal(2, list.Count);
                Assert.Equal(1, list[0].Id);
                Assert.Equal("Data-1", list[0].Name);
                Assert.Equal(2, list[1].Id);
                Assert.Equal("Data-2", list[1].Name);
            }
        }

        [Dao]
        public interface IQuerySimpleAsyncDao
        {
            [Query]
            Task<IList<DataEntity>> QueryBufferdAsync();

            [Query]
            Task<IEnumerable<DataEntity>> QueryNonBufferdAsync();
        }

        [Fact]
        public async Task QueryBufferdSimpleAsync()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("SELECT * FROM Data ORDER BY Id")
                    .Build();
                var dao = generator.Create<IQuerySimpleAsyncDao>();

                var list = await dao.QueryBufferdAsync();

                Assert.Equal(2, list.Count);
                Assert.Equal(1, list[0].Id);
                Assert.Equal("Data-1", list[0].Name);
                Assert.Equal(2, list[1].Id);
                Assert.Equal("Data-2", list[1].Name);
            }
        }

        [Fact]
        public async Task QueryNonBufferdSimpleAsync()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("SELECT * FROM Data ORDER BY Id")
                    .Build();
                var dao = generator.Create<IQuerySimpleAsyncDao>();

                var list = (await dao.QueryNonBufferdAsync()).ToList();

                Assert.Equal(2, list.Count);
                Assert.Equal(1, list[0].Id);
                Assert.Equal("Data-1", list[0].Name);
                Assert.Equal(2, list[1].Id);
                Assert.Equal("Data-2", list[1].Name);
            }
        }

        //--------------------------------------------------------------------------------
        // With Connection
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IQueryWithConnectionDao
        {
            [Query]
            IList<DataEntity> QueryBufferd(DbConnection con);

            [Query]
            IEnumerable<DataEntity> QueryNonBufferd(DbConnection con);
        }

        [Fact]
        public void QueryBufferdWithConnection()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql("SELECT * FROM Data ORDER BY Id")
                    .Build();
                var dao = generator.Create<IQueryWithConnectionDao>();

                con.Open();

                var list = dao.QueryBufferd(con);

                Assert.Equal(ConnectionState.Open, con.State);
                Assert.Equal(2, list.Count);
                Assert.Equal(1, list[0].Id);
                Assert.Equal("Data-1", list[0].Name);
                Assert.Equal(2, list[1].Id);
                Assert.Equal("Data-2", list[1].Name);
            }
        }

        [Fact]
        public void QueryNonBufferdWithConnection()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql("SELECT * FROM Data ORDER BY Id")
                    .Build();
                var dao = generator.Create<IQueryWithConnectionDao>();

                con.Open();

                var list = dao.QueryNonBufferd(con).ToList();

                Assert.Equal(ConnectionState.Open, con.State);
                Assert.Equal(2, list.Count);
                Assert.Equal(1, list[0].Id);
                Assert.Equal("Data-1", list[0].Name);
                Assert.Equal(2, list[1].Id);
                Assert.Equal("Data-2", list[1].Name);
            }
        }

        [Dao]
        public interface IQueryWithConnectionAsyncDao
        {
            [Query]
            Task<IList<DataEntity>> QueryBufferdAsync(DbConnection con);

            [Query]
            Task<IEnumerable<DataEntity>> QueryNonBufferdAsync(DbConnection con);
        }

        [Fact]
        public async Task QueryBufferdWithConnectionAsync()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql("SELECT * FROM Data ORDER BY Id")
                    .Build();
                var dao = generator.Create<IQueryWithConnectionAsyncDao>();

                con.Open();

                var list = await dao.QueryBufferdAsync(con);

                Assert.Equal(ConnectionState.Open, con.State);
                Assert.Equal(2, list.Count);
                Assert.Equal(1, list[0].Id);
                Assert.Equal("Data-1", list[0].Name);
                Assert.Equal(2, list[1].Id);
                Assert.Equal("Data-2", list[1].Name);
            }
        }

        [Fact]
        public async Task QueryNonBufferdWithConnectionAsync()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql("SELECT * FROM Data ORDER BY Id")
                    .Build();
                var dao = generator.Create<IQueryWithConnectionAsyncDao>();

                con.Open();

                var list = (await dao.QueryNonBufferdAsync(con)).ToList();

                Assert.Equal(ConnectionState.Open, con.State);
                Assert.Equal(2, list.Count);
                Assert.Equal(1, list[0].Id);
                Assert.Equal("Data-1", list[0].Name);
                Assert.Equal(2, list[1].Id);
                Assert.Equal("Data-2", list[1].Name);
            }
        }

        //--------------------------------------------------------------------------------
        // Cancel
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IQueryCancelAsyncDao
        {
            [Query]
            Task<IList<DataEntity>> QueryBufferdAsync(CancellationToken cancel);

            [Query]
            Task<IEnumerable<DataEntity>> QueryNonBufferdAsync(CancellationToken cancel);
        }

        [Fact]
        public async Task QueryBufferdCancelAsync()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("SELECT * FROM Data ORDER BY Id")
                    .Build();
                var dao = generator.Create<IQueryCancelAsyncDao>();

                var list = await dao.QueryBufferdAsync(default);

                Assert.Equal(2, list.Count);

                var cancel = new CancellationToken(true);
                await Assert.ThrowsAsync<TaskCanceledException>(async () => await dao.QueryBufferdAsync(cancel));
            }
        }

        [Fact]
        public async Task QueryNonBufferdCancelAsync()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable()
                .InsertData(new DataEntity { Id = 1, Name = "Data-1" })
                .InsertData(new DataEntity { Id = 2, Name = "Data-2" }))
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .UseFileDatabase()
                    .SetSql("SELECT * FROM Data ORDER BY Id")
                    .Build();
                var dao = generator.Create<IQueryCancelAsyncDao>();

                var list = (await dao.QueryNonBufferdAsync(default)).ToList();

                Assert.Equal(2, list.Count);

                var cancel = new CancellationToken(true);
                await Assert.ThrowsAsync<TaskCanceledException>(async () => await dao.QueryNonBufferdAsync(cancel));
            }
        }

        //--------------------------------------------------------------------------------
        // Invalid
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IQueryInvalidDao
        {
            [Query]
            void Query();
        }

        [Fact]
        public void QueryInvalid()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql(string.Empty)
                .Build();

            Assert.Throws<AccessorGeneratorException>(() => generator.Create<IQueryInvalidDao>());
        }

        [Dao]
        public interface IQueryInvalidAsyncDao
        {
            [Query]
            Task QueryAsync();
        }

        [Fact]
        public void QueryInvalidAsync()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql(string.Empty)
                .Build();

            Assert.Throws<AccessorGeneratorException>(() => generator.Create<IQueryInvalidAsyncDao>());
        }
    }
}
