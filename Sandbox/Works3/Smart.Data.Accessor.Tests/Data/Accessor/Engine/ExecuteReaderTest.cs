namespace Smart.Data.Accessor.Engine
{
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;

    using Smart.Data.Accessor.Attributes;
    using Smart.Mock;

    using Xunit;

    public class ExecuteReaderTest
    {
        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IExecuteReaderSimpleDao
        {
            [ExecuteReader]
            IDataReader ExecuteReader();
        }

        [Fact]
        public void ExecuteReaderSimple()
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

                var dao = generator.Create<IExecuteReaderSimpleDao>();

                using (var reader = dao.ExecuteReader())
                {
                    Assert.True(reader.Read());
                    Assert.True(reader.Read());
                    Assert.False(reader.Read());
                }
            }
        }

        [Dao]
        public interface IExecuteReaderSimpleAsyncDao
        {
            [ExecuteReader]
            Task<IDataReader> ExecuteReaderAsync();
        }

        [Fact]
        public async Task ExecuteReaderSimpleAsync()
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

                var dao = generator.Create<IExecuteReaderSimpleAsyncDao>();

                using (var reader = await dao.ExecuteReaderAsync())
                {
                    Assert.True(reader.Read());
                    Assert.True(reader.Read());
                    Assert.False(reader.Read());
                }
            }
        }

        //--------------------------------------------------------------------------------
        // With Connection
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IExecuteReaderWithConnectionDao
        {
            [ExecuteReader]
            IDataReader ExecuteReader(DbConnection con);
        }

        [Fact]
        public void ExecuteReaderWithConnection()
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

                con.Open();

                var dao = generator.Create<IExecuteReaderWithConnectionDao>();

                Assert.Equal(ConnectionState.Open, con.State);

                using (var reader = dao.ExecuteReader(con))
                {
                    Assert.True(reader.Read());
                    Assert.True(reader.Read());
                    Assert.False(reader.Read());
                }

                Assert.Equal(ConnectionState.Open, con.State);
            }
        }

        [Dao]
        public interface IExecuteReaderWithConnectionAsyncDao
        {
            [ExecuteReader]
            Task<IDataReader> ExecuteReaderAsync(DbConnection con);
        }

        [Fact]
        public async Task ExecuteReaderWithConnectionAsync()
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

                con.Open();

                var dao = generator.Create<IExecuteReaderWithConnectionAsyncDao>();

                Assert.Equal(ConnectionState.Open, con.State);

                using (var reader = await dao.ExecuteReaderAsync(con))
                {
                    Assert.True(reader.Read());
                    Assert.True(reader.Read());
                    Assert.False(reader.Read());
                }

                Assert.Equal(ConnectionState.Open, con.State);
            }
        }

        // TODO
    }
}