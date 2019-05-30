namespace WorkGenerated.Tests
{
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper;

    using Xunit;

    public class SampleDaoExecuteReader
    {
        //--------------------------------------------------------------------------------
        // Auto Connection
        //--------------------------------------------------------------------------------

        [Fact]
        public void ExecuteReader()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                using (var reader = dao.ExecuteReader())
                {
                    Assert.True(reader.Read());
                    Assert.False(reader.Read());
                }
            }
        }

        [Fact]
        public async Task ExecuteReaderAsync()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                var cancel = new CancellationToken();
                using (var reader = await dao.ExecuteReaderAsync(cancel).ConfigureAwait(false))
                {
                    Assert.True(reader.Read());
                    Assert.False(reader.Read());
                }
            }
        }

        //--------------------------------------------------------------------------------
        // Manual Connection Open
        //--------------------------------------------------------------------------------

        [Fact]
        public void ExecuteReaderManualWithOpen()
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

                    using (var reader = dao.ExecuteReader(con2))
                    {
                        Assert.True(reader.Read());
                        Assert.False(reader.Read());
                    }

                    Assert.Equal(ConnectionState.Open, con2.State);
                }
            }
        }

        [Fact]
        public void ExecuteReaderManualWithoutOpen()
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
                    using (var reader = dao.ExecuteReader(con2))
                    {
                        Assert.True(reader.Read());
                        Assert.False(reader.Read());
                    }

                    Assert.Equal(ConnectionState.Closed, con2.State);
                }
            }
        }

        [Fact]
        public async Task ExecuteReaderAsyncManualWithOpen()
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
                    using (var reader = await dao.ExecuteReaderAsync(con2, cancel).ConfigureAwait(false))
                    {
                        Assert.True(reader.Read());
                        Assert.False(reader.Read());
                    }

                    Assert.Equal(ConnectionState.Open, con2.State);
                }
            }
        }

        [Fact]
        public async Task ExecuteReaderAsyncManualWithoutOpen()
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
                    using (var reader = await dao.ExecuteReaderAsync(con2, cancel).ConfigureAwait(false))
                    {
                        Assert.True(reader.Read());
                        Assert.False(reader.Read());
                    }

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
                Assert.Throws<SqliteException>(() => dao.ExecuteReader(con));

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

                Assert.Throws<SqliteException>(() => dao.ExecuteReader(con));

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
                await Assert.ThrowsAsync<SqliteException>(async () => await dao.ExecuteReaderAsync(con, cancel));

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
                await Assert.ThrowsAsync<SqliteException>(async () => await dao.ExecuteReaderAsync(con, cancel));

                Assert.Equal(ConnectionState.Open, con.State);
            }
        }
    }
}
