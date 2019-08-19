namespace WorkClient
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;

    using Smart.Data.Accessor.Attributes;
    using Smart.Mock.Data;

    using WorkClient.Mock;

    public static class Program
    {
        public static void Main()
        {
            var generator = new TestFactoryBuilder()
                .UseFileDatabase()
                .SetSql("/*% var value = id; */SELECT * FROM Data WHERE Id = /* @value */1")
                .Build();
            var dao = generator.Create<ITestDao>();

            var con = new MockDbConnection();
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Debug.Assert(c.Parameters[0].DbType == DbType.Int32);
                    Debug.Assert((int)c.Parameters[0].Value == 1);
                };
                cmd.SetupResult(1);
            });
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Debug.Assert(c.Parameters[0].Value == DBNull.Value);
                };
                cmd.SetupResult(0);
            });
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Debug.Assert(c.Parameters[0].DbType == DbType.String);
                    Debug.Assert(c.Parameters[0].Value == "x");
                };
                cmd.SetupResult(1);
            });

            dao.Execute(con, 1);
            dao.Execute(con, null);
            dao.Execute(con, "x");

            // TODO foreach
        }
    }

    [DataAccessor]
    public interface ITestDao
    {
        [Execute]
        int Execute(DbConnection con, object id);
    }

    public class DataEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
