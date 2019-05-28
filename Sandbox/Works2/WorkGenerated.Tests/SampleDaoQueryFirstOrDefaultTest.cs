using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Smart.Data.Mapper;
using Xunit;

namespace WorkGenerated.Tests
{
    public class SampleDaoQueryFirstOrDefaultTest
    {
        //--------------------------------------------------------------------------------
        // Auto Connection
        //--------------------------------------------------------------------------------

        [Fact]

        public void QueryFirstOrDefault()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                var entity = dao.QueryFirstOrDefault();

                Assert.NotNull(entity);
            }
        }

        [Fact]

        public async Task QueryFirstOrDefaultAsync()
        {
            using (var con = new SqliteConnection(Connections.Memory))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                // Test
                var dao = DaoFactory.CreateSampleDao(() => new SqliteConnection(Connections.Memory));

                var cancel = new CancellationToken();
                var entity = await dao.QueryFirstOrDefaultAsync(cancel).ConfigureAwait(false);

                Assert.NotNull(entity);
            }
        }

        //--------------------------------------------------------------------------------
        // Manual Connection
        //--------------------------------------------------------------------------------

        [Fact]

        public void QueryFirstOrDefaultManualWithOpen()
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

                    var entity = dao.QueryFirstOrDefault(con2);

                    Assert.NotNull(entity);

                    Assert.Equal(ConnectionState.Open, con2.State);
                }
            }
        }

        [Fact]

        public void QueryFirstOrDefaultManualWithoutOpen()
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
                    var entity = dao.QueryFirstOrDefault(con2);

                    Assert.NotNull(entity);

                    Assert.Equal(ConnectionState.Closed, con2.State);
                }
            }
        }

        [Fact]

        public async Task QueryFirstOrDefaultAsyncManualWithOpen()
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
                    var entity = await dao.QueryFirstOrDefaultAsync(con2, cancel).ConfigureAwait(false);

                    Assert.NotNull(entity);

                    Assert.Equal(ConnectionState.Open, con2.State);
                }
            }
        }

        [Fact]

        public async Task QueryFirstOrDefaultAsyncManualWithoutOpen()
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
                    var entity = await dao.QueryFirstOrDefaultAsync(con2, cancel).ConfigureAwait(false);

                    Assert.NotNull(entity);

                    Assert.Equal(ConnectionState.Closed, con2.State);
                }
            }
        }
    }
}
