namespace Smart.Data.Accessor
{
    using System;
    using System.Collections.Generic;
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
        public interface IAnsiStringAttributeDao
        {
            [Execute]
            int Execute(DbConnection con, [AnsiString(3)] string id1, [AnsiString] string id2);
        }

        [Fact]
        public void UseAnsiStringAttribute()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql("Id1 = /*@ id1 */'xxx' AND Id2 = /*@ id2 */'a'")
                .Build();

            var dao = generator.Create<IAnsiStringAttributeDao>();

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
        public interface IDbTypeAttributeDao
        {
            [Execute]
            int Execute(DbConnection con, [DbType(DbType.AnsiStringFixedLength, 3)] string id1, [DbType(DbType.AnsiString)] string id2);
        }

        [Fact]
        public void UseDbTypeAttribute()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql("Id1 = /*@ id1 */'xxx' AND Id2 = /*@ id2 */'a'")
                .Build();

            var dao = generator.Create<IDbTypeAttributeDao>();

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

        //--------------------------------------------------------------------------------
        // TypeHandler
        //--------------------------------------------------------------------------------

        public class DummyTypeHandler : ITypeHandler
        {
            public void SetValue(DbParameter parameter, object value) => parameter.Size = 5;

            public Func<object, object> CreateParse(Type type) => x => x;
        }

        [Dao]
        public interface ITypeHandlerDao
        {
            [Execute]
            void ExecuteIn(DbConnection con, string value);

            [Execute]
            void ExecuteInOut(DbConnection con, ref string value);

            [Execute]
            void ExecuteArray(DbConnection con, string[] value);

            [Execute]
            void ExecuteList(DbConnection con, List<string> value);
        }

        [Fact]
        public void UseTypeHandler()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql("/*@ value */")
                .Config(config =>
                {
                    config.ConfigureTypeHandlers(handlers =>
                    {
                        handlers[typeof(string)] = new DummyTypeHandler();
                    });
                })
                .Build();

            var dao = generator.Create<ITypeHandlerDao>();

            var con = new MockDbConnection();

            // IN
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c => Assert.Equal(5, c.Parameters[0].Size);
                cmd.SetupResult(0);
            });

            dao.ExecuteIn(con, "x");

            con.SetupCommand(cmd =>
            {
                cmd.Executing = c => Assert.Equal(DBNull.Value, c.Parameters[0].Value);
                cmd.SetupResult(0);
            });

            dao.ExecuteIn(con, null);

            // IN/OUT
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c => Assert.Equal(5, c.Parameters[0].Size);
                cmd.SetupResult(0);
            });

            var value = "x";
            dao.ExecuteInOut(con, ref value);

            con.SetupCommand(cmd =>
            {
                cmd.Executing = c => Assert.Equal(DBNull.Value, c.Parameters[0].Value);
                cmd.SetupResult(0);
            });

            value = null;
            dao.ExecuteInOut(con, ref value);

            // Array
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(5, c.Parameters[0].Size);
                    Assert.Equal(DBNull.Value, c.Parameters[1].Value);
                };
                cmd.SetupResult(0);
            });

            dao.ExecuteArray(con, new[] { string.Empty, null });

            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(0, c.Parameters.Count);
                };
                cmd.SetupResult(0);
            });

            dao.ExecuteArray(con, null);

            // List
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(5, c.Parameters[0].Size);
                    Assert.Equal(DBNull.Value, c.Parameters[1].Value);
                };
                cmd.SetupResult(0);
            });

            dao.ExecuteList(con, new List<string> { string.Empty, null });

            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(0, c.Parameters.Count);
                };
                cmd.SetupResult(0);
            });

            dao.ExecuteList(con, null);
        }

        //--------------------------------------------------------------------------------
        // DbType
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IDbTypeDao
        {
            [Execute]
            void ExecuteIn(DbConnection con, [DbType(DbType.AnsiStringFixedLength, 5)] string value);

            [Execute]
            void ExecuteInOut(DbConnection con, [DbType(DbType.AnsiStringFixedLength, 5)] ref string value);

            [Execute]
            void ExecuteOut(DbConnection con, [DbType(DbType.AnsiStringFixedLength, 5)] out string value);

            [Execute]
            void ExecuteArray(DbConnection con, [DbType(DbType.AnsiStringFixedLength, 5)] string[] value);

            [Execute]
            void ExecuteList(DbConnection con, [DbType(DbType.AnsiStringFixedLength, 5)] List<string> value);
        }

        [Fact]
        public void UseDbType()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql("/*@ value */")
                .Config(config =>
                {
                    config.ConfigureTypeHandlers(handlers =>
                    {
                        handlers[typeof(string)] = new DummyTypeHandler();
                    });
                })
                .Build();

            var dao = generator.Create<IDbTypeDao>();

            var con = new MockDbConnection();

            // IN
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c => Assert.Equal(5, c.Parameters[0].Size);
                cmd.SetupResult(0);
            });

            dao.ExecuteIn(con, "x");

            con.SetupCommand(cmd =>
            {
                cmd.Executing = c => Assert.Equal(DBNull.Value, c.Parameters[0].Value);
                cmd.SetupResult(0);
            });

            dao.ExecuteIn(con, null);

            // IN/OUT
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c => Assert.Equal(5, c.Parameters[0].Size);
                cmd.SetupResult(0);
            });

            var value = "x";
            dao.ExecuteInOut(con, ref value);

            con.SetupCommand(cmd =>
            {
                cmd.Executing = c => Assert.Equal(DBNull.Value, c.Parameters[0].Value);
                cmd.SetupResult(0);
            });

            value = null;
            dao.ExecuteInOut(con, ref value);

            // OUT
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c => Assert.Equal(5, c.Parameters[0].Size);
                cmd.SetupResult(0);
            });

            dao.ExecuteOut(con, out _);

            // Array
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(5, c.Parameters[0].Size);
                    Assert.Equal(DBNull.Value, c.Parameters[1].Value);
                };
                cmd.SetupResult(0);
            });

            dao.ExecuteArray(con, new[] { string.Empty, null });

            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(0, c.Parameters.Count);
                };
                cmd.SetupResult(0);
            });

            dao.ExecuteArray(con, null);

            // List
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(5, c.Parameters[0].Size);
                    Assert.Equal(DBNull.Value, c.Parameters[1].Value);
                };
                cmd.SetupResult(0);
            });

            dao.ExecuteList(con, new List<string> { string.Empty, null });

            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(0, c.Parameters.Count);
                };
                cmd.SetupResult(0);
            });

            dao.ExecuteList(con, null);
        }

        //--------------------------------------------------------------------------------
        // DbType
        //--------------------------------------------------------------------------------

        [Dao]
        public interface ISetupFailedInDao
        {
            [Execute]
            void ExecuteIn(DbConnection con, string value);
        }

        [Fact]
        public void SetupFailedIn()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql("/*@ value */")
                .Config(config =>
                {
                    config.ConfigureTypeMap(map => map.Clear());
                })
                .Build();

            Assert.Throws<AccessorRuntimeException>(() => generator.Create<ISetupFailedInDao>());
        }

        [Dao]
        public interface ISetupFailedInOutDao
        {
            [Execute]
            void ExecuteInOut(DbConnection con, ref string value);
        }

        [Fact]
        public void SetupFailedInOut()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql("/*@ value */")
                .Config(config =>
                {
                    config.ConfigureTypeMap(map => map.Clear());
                })
                .Build();

            Assert.Throws<AccessorRuntimeException>(() => generator.Create<ISetupFailedInOutDao>());
        }

        [Dao]
        public interface ISetupFailedOutDao
        {
            [Execute]
            void ExecuteOut(DbConnection con, out string value);
        }

        [Fact]
        public void SetupFailedOut()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql("/*@ value */")
                .Config(config =>
                {
                    config.ConfigureTypeMap(map => map.Clear());
                })
                .Build();

            Assert.Throws<AccessorRuntimeException>(() => generator.Create<ISetupFailedOutDao>());
        }

        [Dao]
        public interface ISetupFailedArrayDao
        {
            [Execute]
            void ExecuteArray(DbConnection con, string[] value);
        }

        [Fact]
        public void SetupFailedArray()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql("/*@ value */")
                .Config(config =>
                {
                    config.ConfigureTypeMap(map => map.Clear());
                })
                .Build();

            Assert.Throws<AccessorRuntimeException>(() => generator.Create<ISetupFailedArrayDao>());
        }

        [Dao]
        public interface ISetupFailedListDao
        {
            [Execute]
            void ExecuteList(DbConnection con, List<string> value);
        }

        [Fact]
        public void SetupFailedList()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql("/*@ value */")
                .Config(config =>
                {
                    config.ConfigureTypeMap(map => map.Clear());
                })
                .Build();

            Assert.Throws<AccessorRuntimeException>(() => generator.Create<ISetupFailedListDao>());
        }
    }
}
