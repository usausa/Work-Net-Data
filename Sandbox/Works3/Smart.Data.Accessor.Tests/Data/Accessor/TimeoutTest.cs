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
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql(string.Empty)
                .Build();

            var dao = generator.Create<ITimeoutAttributeDao>();

            var con = new MockDbConnection();
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(123, c.CommandTimeout);
                };
                cmd.SetupResult(1);
            });

            dao.Execute(con);
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
            var generator = new GeneratorBuilder()
                .EnableDebug()
                .SetSql(string.Empty)
                .Build();

            var dao = generator.Create<ITimeoutParameterDao>();

            var con = new MockDbConnection();
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal(123, c.CommandTimeout);
                };
                cmd.SetupResult(1);
            });

            dao.Execute(con, 123);
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