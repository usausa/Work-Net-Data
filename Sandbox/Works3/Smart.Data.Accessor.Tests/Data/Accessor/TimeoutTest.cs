namespace Smart.Data.Accessor
{
    using System.Data.Common;

    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Generator;
    using Smart.Mock;
    using Smart.Mock.Data;

    using Xunit;

    public class TimeoutTest
    {
        [Dao]
        public interface ITimeoutAttributeDao
        {
            [Execute]
            [Timeout(123)]
            int Execute(DbConnection con);
        }

        [Fact]
        public void TimeoutAttribute()
        {
            using (TestDatabase.Initialize())
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql(string.Empty)
                    .Build();

                var dao = generator.Create<ITimeoutAttributeDao>();

                var cmd = new MockDbCommand
                {
                    Executing = c =>
                    {
                        Assert.Equal(123, c.CommandTimeout);
                    }
                };
                cmd.SetupResult(1);
                var con = new MockDbConnection();
                con.SetupCommand(cmd);

                dao.Execute(con);
            }
        }

        [Dao]
        public interface ITimeoutParameterDao
        {
            [Execute]
            int Execute(DbConnection con, [TimeoutParameter] int timeout);
        }

        [Fact]
        public void TimeoutParameter()
        {
            using (TestDatabase.Initialize())
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql(string.Empty)
                    .Build();

                var dao = generator.Create<ITimeoutParameterDao>();

                var cmd = new MockDbCommand
                {
                    Executing = c =>
                    {
                        Assert.Equal(123, c.CommandTimeout);
                    }
                };
                cmd.SetupResult(1);
                var con = new MockDbConnection();
                con.SetupCommand(cmd);

                dao.Execute(con, 123);
            }
        }

        [Dao]
        public interface IInvalidTimeoutParameterDao
        {
            [Execute]
            int Execute(DbConnection con, [TimeoutParameter] string timeout);
        }

        [Fact]
        public void InvalidTimeoutParameter()
        {
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql(string.Empty)
                .Build();

            Assert.Throws<AccessorGeneratorException>(() => generator.Create<IInvalidTimeoutParameterDao>());
        }
    }
}
