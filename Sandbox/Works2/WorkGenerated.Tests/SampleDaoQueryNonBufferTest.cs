namespace WorkGenerated.Tests
{
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper;

    using Xunit;

    public class SampleDaoQueryNonBufferTest
    {
        //--------------------------------------------------------------------------------
        // Auto Connection
        //--------------------------------------------------------------------------------

        [Fact]
        public void QueryNonBuffer()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                var list = dao.QueryNonBuffer().ToList();

                Assert.Single(list);
            }
        }

        [Fact]
        public async Task QueryNonBufferAsync()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                var cancel = new CancellationToken();
                var list = (await dao.QueryNonBufferAsync(cancel).ConfigureAwait(false)).ToList();

                Assert.Single(list);
            }
        }

        //--------------------------------------------------------------------------------
        // Manual Connection
        //--------------------------------------------------------------------------------

        [Fact]
        public void QueryNonBufferManualWithOpen()
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

                    var list = dao.QueryNonBuffer(con2);

                    Assert.Equal(ConnectionState.Open, con2.State);

                    Assert.Single(list.ToList());

                    Assert.Equal(ConnectionState.Open, con2.State);
                }
            }
        }

        [Fact]
        public void QueryNonBufferManualWithoutOpen()
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
                    var list = dao.QueryNonBuffer(con2).ToList();

                    Assert.Single(list);

                    Assert.Equal(ConnectionState.Closed, con2.State);
                }
            }
        }

        [Fact]
        public async Task QueryNonBufferAsyncManualWithOpen()
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
                    var list = await dao.QueryNonBufferAsync(con2, cancel).ConfigureAwait(false);

                    Assert.Equal(ConnectionState.Open, con2.State);

                    Assert.Single(list.ToList());

                    Assert.Equal(ConnectionState.Open, con2.State);
                }
            }
        }

        [Fact]
        public async Task QueryNonBufferAsyncManualWithoutOpen()
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
                    var list = (await dao.QueryNonBufferAsync(con2, cancel).ConfigureAwait(false)).ToList();

                    Assert.Single(list);

                    Assert.Equal(ConnectionState.Closed, con2.State);
                }
            }
        }

        //--------------------------------------------------------------------------------
        // Manual Connection Close
        //--------------------------------------------------------------------------------

        [Fact]
        public void ClosedConnectionMustClosedWhenQueryError()
        {
            var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
            using (var con = new SqliteConnection(Connections.Memory))
            {
                Assert.Throws<SqliteException>(() => dao.QueryNonBuffer(con).ToList());

                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        [Fact]
        public void OpenedConnectionMustOpenedWhenQueryError()
        {
            var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();

                Assert.Throws<SqliteException>(() => dao.QueryNonBuffer(con).ToList());

                Assert.Equal(ConnectionState.Open, con.State);
            }
        }

        [Fact]
        public async Task ClosedConnectionMustClosedWhenQueryErrorAsync()
        {
            var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
            using (var con = new SqliteConnection(Connections.Memory))
            {
                var cancel = new CancellationToken();
                await Assert.ThrowsAsync<SqliteException>(async () => await dao.QueryNonBufferAsync(con, cancel));

                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        [Fact]
        public async Task OpenedConnectionMustOpenedWhenQueryErrorAsync()
        {
            var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();

                var cancel = new CancellationToken();
                await Assert.ThrowsAsync<SqliteException>(async () => await dao.QueryNonBufferAsync(con, cancel));

                Assert.Equal(ConnectionState.Open, con.State);
            }
        }
    }
}
