namespace Smart.Mock
{
    using System.Data.Common;

    using Smart.Data.Mapper;

    public static class DbConnectionExtensions
    {
        public static DbConnection SetupDataTable(this DbConnection con)
        {
            con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
            return con;
        }

        public static DataEntity QueryData(this DbConnection con, long id)
        {
            return con.QueryFirstOrDefault<DataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = id });
        }

        // TODO
    }
}
