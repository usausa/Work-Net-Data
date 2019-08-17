namespace WorkClient
{
    using System.Data.Common;

    using Smart.Data.Accessor.Attributes;
    using Smart.Mock.Data;

    using WorkClient.Mock;

    public static class Program
    {
        public static void Main()
        {
            var generator = new TestFactoryBuilder()
                .UseFileDatabase()
                .SetSql("INSERT INTO Data (Id, Name) VALUES (/*@ Id */1, /*@ Name */'test')")
                .Build();
            var dao = generator.Create<ITestDao>();

            var con = new MockDbConnection();
            con.SetupCommand(cmd =>
            {
                cmd.SetupResult(1);
            });

            dao.Insert(con, new DataEntity { Id = 1, Name = "test" });
        }
    }

    [DataAccessor]
    public interface ITestDao
    {
        [Execute]
        int Insert(DbConnection con, DataEntity entity);
    }

    public class DataEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
