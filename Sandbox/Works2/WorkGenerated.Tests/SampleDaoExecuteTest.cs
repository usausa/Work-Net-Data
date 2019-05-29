namespace WorkGenerated.Tests
{
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper;

    using Xunit;

    public class SampleDaoExecuteTest
    {
        //--------------------------------------------------------------------------------
        // Auto Connection
        //--------------------------------------------------------------------------------

        [Fact]
        public void Execute()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                dao.Execute();

                var count = con.ExecuteScalar<long>("SELECT * FROM Data");
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task ExecuteAsync()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                var cancel = new CancellationToken();
                await dao.ExecuteAsync(cancel).ConfigureAwait(false);

                var count = con.ExecuteScalar<long>("SELECT * FROM Data");
                Assert.Equal(1, count);
            }
        }

        //--------------------------------------------------------------------------------
        // Manual Connection
        //--------------------------------------------------------------------------------

        [Fact]
        public void ExecuteManualWithOpen()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
                using (var con2 = new SqliteConnection(Connections.Memory))
                {
                    con2.Open();

                    dao.Execute(con2);

                    Assert.Equal(ConnectionState.Open, con2.State);
                }

                var count = con.ExecuteScalar<long>("SELECT * FROM Data");
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public void ExecuteManualWithoutOpen()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
                using (var con2 = new SqliteConnection(Connections.Memory))
                {
                    dao.Execute(con2);

                    Assert.Equal(ConnectionState.Closed, con2.State);
                }

                var count = con.ExecuteScalar<long>("SELECT * FROM Data");
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task ExecuteAsyncManualWithOpen()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
                using (var con2 = new SqliteConnection(Connections.Memory))
                {
                    con2.Open();

                    var cancel = new CancellationToken();
                    await dao.ExecuteAsync(con2, cancel);

                    Assert.Equal(ConnectionState.Open, con2.State);
                }

                var count = con.ExecuteScalar<long>("SELECT * FROM Data");
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task ExecuteAsyncManualWithoutOpen()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
                using (var con2 = new SqliteConnection(Connections.Memory))
                {
                    var cancel = new CancellationToken();
                    await dao.ExecuteAsync(con2, cancel);

                    Assert.Equal(ConnectionState.Closed, con2.State);
                }

                var count = con.ExecuteScalar<long>("SELECT * FROM Data");
                Assert.Equal(1, count);
            }
        }
    }
}
