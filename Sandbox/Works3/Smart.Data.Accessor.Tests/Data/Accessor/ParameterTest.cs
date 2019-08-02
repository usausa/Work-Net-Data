namespace Smart.Data.Accessor
{
    using System.Data;
    using System.Data.Common;

    using Smart.Data.Accessor.Attributes;
    using Smart.Mock;
    using Smart.Mock.Data;

    using Xunit;

    public class ParameterTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IAnsiStringDao
        {
            [Execute]
            int Execute(DbConnection con, [AnsiString(3)] string id1, [AnsiString] string id2);
        }

        [Fact]
        public void UseAnsiString()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql("Id1 = /*@ id1 */'xxx' AND Id2 = /*@ id2 */'a'")
                .Build();

            var dao = generator.Create<IAnsiStringDao>();

            var cmd = new MockDbCommand
            {
                Executing = c =>
                {
                    Assert.Equal(DbType.AnsiStringFixedLength, c.Parameters[0].DbType);
                    Assert.Equal(3, c.Parameters[0].Size);
                    Assert.Equal(DbType.AnsiString, c.Parameters[1].DbType);
                }
            };
            cmd.SetupResult(1);
            var con = new MockDbConnection();
            con.SetupCommand(cmd);

            dao.Execute(con, "xxx", "a");
        }

        [Dao]
        public interface IDbTypeDao
        {
            [Execute]
            int Execute(DbConnection con, [DbType(DbType.AnsiStringFixedLength, 3)] string id1, [DbType(DbType.AnsiString)] string id2);
        }

        [Fact]
        public void UseDbType()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql("Id1 = /*@ id1 */'xxx' AND Id2 = /*@ id2 */'a'")
                .Build();

            var dao = generator.Create<IDbTypeDao>();

            var cmd = new MockDbCommand
            {
                Executing = c =>
                {
                    Assert.Equal(DbType.AnsiStringFixedLength, c.Parameters[0].DbType);
                    Assert.Equal(3, c.Parameters[0].Size);
                    Assert.Equal(DbType.AnsiString, c.Parameters[1].DbType);
                }
            };
            cmd.SetupResult(1);
            var con = new MockDbConnection();
            con.SetupCommand(cmd);

            dao.Execute(con, "xxx", "a");
        }

        //--------------------------------------------------------------------------------
        // TypeHandler
        //--------------------------------------------------------------------------------

        // TODO
    }
}
