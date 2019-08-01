namespace Smart.Data.Accessor
{
    using System.Data.Common;
    using System.Threading.Tasks;

    using Smart.Data.Accessor.Attributes;
    using Smart.Mock;

    using Xunit;

    public class TransactionTest
    {
        [Dao]
        public interface ITransactionDao
        {
            [Execute]
            int Execute(DbTransaction tx, long id, string name);
        }

        [Fact]
        public void Transaction()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable())
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql("INSERT INTO Data (Id, Name) VALUES (/*@ id */1, /*@ name */'test')")
                    .Build();
                var dao = generator.Create<ITransactionDao>();

                con.Open();

                using (var tx = con.BeginTransaction())
                {
                    var effect = dao.Execute(tx, 1L, "xxx");
                    Assert.Equal(1, effect);

                    tx.Rollback();
                }

                var entity = con.QueryData(1L);
                Assert.Null(entity);

                using (var tx = con.BeginTransaction())
                {
                    var effect = dao.Execute(tx, 1L, "xxx");
                    Assert.Equal(1, effect);

                    tx.Commit();
                }

                entity = con.QueryData(1L);
                Assert.NotNull(entity);
            }
        }

        [Dao]
        public interface ITransactionAsyncDao
        {
            [Execute]
            Task<int> ExecuteAsync(DbTransaction tx, long id, string name);
        }

        [Fact]
        public async Task TransactionAsync()
        {
            using (var con = TestDatabase.Initialize()
                .SetupDataTable())
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .SetSql("INSERT INTO Data (Id, Name) VALUES (/*@ id */1, /*@ name */'test')")
                    .Build();
                var dao = generator.Create<ITransactionAsyncDao>();

                con.Open();

                using (var tx = con.BeginTransaction())
                {
                    var effect = await dao.ExecuteAsync(tx, 1L, "xxx");
                    Assert.Equal(1, effect);

                    tx.Rollback();
                }

                var entity = con.QueryData(1L);
                Assert.Null(entity);

                using (var tx = con.BeginTransaction())
                {
                    var effect = await dao.ExecuteAsync(tx, 1L, "xxx");
                    Assert.Equal(1, effect);

                    tx.Commit();
                }

                entity = con.QueryData(1L);
                Assert.NotNull(entity);
            }
        }
    }
}