namespace WorkGenerated.Tests
{
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper;

    using Xunit;

    public class SampleDaoQueryBufferTest
    {
        //--------------------------------------------------------------------------------
        // Auto Connection
        //--------------------------------------------------------------------------------

        [Fact]
        public void QueryBuffer()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                var list = dao.QueryBuffer();

                Assert.Equal(1, list.Count);
            }
        }

        [Fact]
        public async Task QueryBufferAsync()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                var cancel = new CancellationToken();
                var list = await dao.QueryBufferAsync(cancel).ConfigureAwait(false);

                Assert.Equal(1, list.Count);
            }
        }

        //--------------------------------------------------------------------------------
        // Manual Connection
        //--------------------------------------------------------------------------------

        [Fact]
        public void QueryBufferManualWithOpen()
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

                    var list = dao.QueryBuffer(con2);

                    Assert.Equal(1, list.Count);

                    Assert.Equal(ConnectionState.Open, con2.State);
                }
            }
        }

        [Fact]
        public void QueryBufferManualWithoutOpen()
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
                    var entity = dao.QueryBuffer(con2);

                    Assert.NotNull(entity);

                    Assert.Equal(ConnectionState.Closed, con2.State);
                }
            }
        }

        [Fact]
        public async Task QueryBufferAsyncManualWithOpen()
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
                    var list = await dao.QueryBufferAsync(con2, cancel).ConfigureAwait(false);

                    Assert.Equal(1, list.Count);

                    Assert.Equal(ConnectionState.Open, con2.State);
                }
            }
        }

        [Fact]
        public async Task QueryBufferAsyncManualWithoutOpen()
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
                    var list = await dao.QueryBufferAsync(con2, cancel).ConfigureAwait(false);

                    Assert.Equal(1, list.Count);

                    Assert.Equal(ConnectionState.Closed, con2.State);
                }
            }
        }
    }
}
