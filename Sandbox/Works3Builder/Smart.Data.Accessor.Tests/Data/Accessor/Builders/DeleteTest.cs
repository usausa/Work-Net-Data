namespace Smart.Data.Accessor.Builders
{
    using System.Data.Common;

    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Attributes.Builders;
    using Smart.Mock;
    using Smart.Mock.Data;

    using Xunit;

    public class DeleteTest
    {
        //--------------------------------------------------------------------------------
        // Key
        //--------------------------------------------------------------------------------

        public class DataEntity
        {
            [Key(1)]
            public long Key1 { get; set; }

            [Key(2)]
            public long Key2 { get; set; }

            public string Name { get; set; }
        }

        [DataAccessor]
        public interface IDeleteByKeyDao
        {
            [Delete]
            void Delete(DbConnection con, DataEntity entity);
        }

        [Fact]
        public void TestDeleteByKey()
        {
            var generator = new TestFactoryBuilder()
                .Build();
            var dao = generator.Create<IDeleteByKeyDao>();

            var con = new MockDbConnection();
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal("DELETE FROM Data WHERE Key1 = @_p0 AND Key2 = @_p1", c.CommandText);
                    Assert.Equal(2, c.Parameters.Count);
                    Assert.Equal(1L, c.Parameters[0].Value);
                    Assert.Equal(2L, c.Parameters[1].Value);
                };
                cmd.SetupResult(1);
            });

            dao.Delete(con, new DataEntity { Key1 = 1L, Key2 = 2L });
        }

        //--------------------------------------------------------------------------------
        // Argument
        //--------------------------------------------------------------------------------

        [DataAccessor]
        public interface IDeleteByArgumentDao
        {
            [Delete("Data")]
            void Delete(DbConnection con, long key1, [Condition(Operand.GreaterEqualThan)] long key2);
        }

        [Fact]
        public void TestDeleteByArgument()
        {
            var generator = new TestFactoryBuilder()
                .Build();
            var dao = generator.Create<IDeleteByArgumentDao>();

            var con = new MockDbConnection();
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal("DELETE FROM Data WHERE Key1 = @_p0 AND Key2 >= @_p1", c.CommandText);
                    Assert.Equal(2, c.Parameters.Count);
                    Assert.Equal(1L, c.Parameters[0].Value);
                    Assert.Equal(2L, c.Parameters[1].Value);
                };
                cmd.SetupResult(1);
            });

            dao.Delete(con, 1L, 2L);
        }

        //--------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------

        public class Parameter
        {
            public long Key1 { get; set; }

            [Condition(Operand.GreaterEqualThan)]
            public long Key2 { get; set; }
        }

        [DataAccessor]
        public interface IDeleteByParameterDao
        {
            [Delete("Data")]
            void Delete(DbConnection con, Parameter parameter);
        }

        [Fact]
        public void TestDeleteByParameter()
        {
            var generator = new TestFactoryBuilder()
                .Build();
            var dao = generator.Create<IDeleteByParameterDao>();

            var con = new MockDbConnection();
            con.SetupCommand(cmd =>
            {
                cmd.Executing = c =>
                {
                    Assert.Equal("DELETE FROM Data WHERE Key1 = @_p0 AND Key2 >= @_p1", c.CommandText);
                    Assert.Equal(2, c.Parameters.Count);
                    Assert.Equal(1L, c.Parameters[0].Value);
                    Assert.Equal(2L, c.Parameters[1].Value);
                };
                cmd.SetupResult(1);
            });

            dao.Delete(con, new Parameter { Key1 = 1L, Key2 = 2L });
        }
    }
}
