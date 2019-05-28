using Microsoft.Data.Sqlite;
using Smart.Data.Mapper;
using Xunit;

namespace WorkGenerated.Tests
{
    public class SampleDaoQueryFirstOrDefaultTest
    {
        //--------------------------------------------------------------------------------
        // Query
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
                Assert.Equal(1, entity.Id);
                Assert.Equal("test1", entity.Name);
            }
        }
    }
}
