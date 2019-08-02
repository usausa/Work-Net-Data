namespace Smart.Data.Accessor
{
    using System;
    using System.Data;
    using System.Data.Common;

    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Handlers;
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

            var con = new MockDbConnection();
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(DbType.AnsiStringFixedLength, c.Parameters[0].DbType);
                    Assert.Equal(3, c.Parameters[0].Size);
                    Assert.Equal(DbType.AnsiString, c.Parameters[1].DbType);
                };
                cmd.SetupResult(1);
            });

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

            var con = new MockDbConnection();
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(DbType.AnsiStringFixedLength, c.Parameters[0].DbType);
                    Assert.Equal(3, c.Parameters[0].Size);
                    Assert.Equal(DbType.AnsiString, c.Parameters[1].DbType);
                };
                cmd.SetupResult(1);
            });

            dao.Execute(con, "xxx", "a");
        }

        //--------------------------------------------------------------------------------
        // TypeHandler
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IDateTimeKindTypeHandlerDao
        {
            [Execute]
            void Execute(DbConnection con, DateTime value);

            [ExecuteScalar]
            DateTime ExecuteScalar(DbConnection con);
        }

        [Fact]
        public void UseDateTimeKindTypeHandler()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql(map =>
                {
                    map[nameof(IDateTimeKindTypeHandlerDao.Execute)] = "/*@ value */";
                    map[nameof(IDateTimeKindTypeHandlerDao.ExecuteScalar)] = string.Empty;
                })
                .Config(config =>
                {
                    config.ConfigureTypeHandlers(handlers =>
                    {
                        handlers[typeof(DateTime)] = new DateTimeKindTypeHandler(DateTimeKind.Local);
                    });
                })
                .Build();

            var dao = generator.Create<IDateTimeKindTypeHandlerDao>();

            var con = new MockDbConnection();
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(DbType.DateTime, c.Parameters[0].DbType);
                };
                cmd.SetupResult(0);
            });
            con.SetupCommand(cmd =>
            {
                cmd.SetupResult(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified));
            });

            dao.Execute(con, new DateTime(2000, 1, 1));

            var result = dao.ExecuteScalar(con);

            Assert.Equal(DateTimeKind.Local, result.Kind);
        }

        [Dao]
        public interface IDateTimeTickTypeHandlerDao
        {
            [Execute]
            void Execute(DbConnection con, DateTime value);

            [ExecuteScalar]
            DateTime ExecuteScalar(DbConnection con);
        }

        [Fact]
        public void UseDateTimeTickTypeHandler()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql(map =>
                {
                    map[nameof(IDateTimeKindTypeHandlerDao.Execute)] = "/*@ value */";
                    map[nameof(IDateTimeKindTypeHandlerDao.ExecuteScalar)] = string.Empty;
                })
                .Config(config =>
                {
                    config.ConfigureTypeHandlers(handlers =>
                    {
                        handlers[typeof(DateTime)] = new DateTimeTickTypeHandler();
                    });
                })
                .Build();

            var dao = generator.Create<IDateTimeTickTypeHandlerDao>();

            var date = new DateTime(2000, 1, 1);

            var con = new MockDbConnection();
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(date.Ticks, c.Parameters[0].Value);
                };
                cmd.SetupResult(0);
            });
            con.SetupCommand(cmd =>
            {
                cmd.SetupResult(date.Ticks);
            });

            dao.Execute(con, new DateTime(2000, 1, 1));

            var result = dao.ExecuteScalar(con);

            Assert.Equal(date, result);
        }
    }
}
