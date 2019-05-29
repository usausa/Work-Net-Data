namespace WorkGenerated.Tests
{
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper;

    using Xunit;

    public class SampleDaoExecuteScalarTest
    {
        //--------------------------------------------------------------------------------
        // Auto Connection
        //--------------------------------------------------------------------------------

        [Fact]
        public void ExecuteScalar()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                var count = dao.ExecuteScalar();

                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async Task ExecuteScalarAsync()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                var cancel = new CancellationToken();
                var count = await dao.ExecuteScalarAsync(cancel).ConfigureAwait(false);

                Assert.Equal(1, count);
            }
        }

        //--------------------------------------------------------------------------------
        // Manual Connection
        //--------------------------------------------------------------------------------

        [Fact]
        public void ExecuteScalarManualWithOpen()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
                using (var con2 = new SqliteConnection(Connections.Memory))
                {
                    con2.Open();

                    var count = dao.ExecuteScalar(con2);

                    Assert.Equal(1, count);

                    Assert.Equal(ConnectionState.Open, con2.State);
                }
            }
        }

        [Fact]
        public void ExecuteScalarManualWithoutOpen()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
                using (var con2 = new SqliteConnection(Connections.Memory))
                {
                    var count = dao.ExecuteScalar(con2);

                    Assert.Equal(1, count);

                    Assert.Equal(ConnectionState.Closed, con2.State);
                }
            }
        }

        [Fact]
        public async Task ExecuteScalarAsyncManualWithOpen()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
                using (var con2 = new SqliteConnection(Connections.Memory))
                {
                    con2.Open();

                    var cancel = new CancellationToken();
                    var count = await dao.ExecuteScalarAsync(con2, cancel).ConfigureAwait(false);

                    Assert.Equal(1, count);

                    Assert.Equal(ConnectionState.Open, con2.State);
                }
            }
        }

        [Fact]
        public async Task ExecuteScalarAsyncManualWithoutOpen()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
                using (var con2 = new SqliteConnection(Connections.Memory))
                {
                    var cancel = new CancellationToken();
                    var count = await dao.ExecuteScalarAsync(con2, cancel).ConfigureAwait(false);

                    Assert.Equal(1, count);

                    Assert.Equal(ConnectionState.Closed, con2.State);
                }
            }
        }
    }
}
