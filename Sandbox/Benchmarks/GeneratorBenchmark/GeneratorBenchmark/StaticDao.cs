namespace GeneratorBenchmark
{
    using Smart.Data;

    public sealed class StaticDao : IDao
    {
        private readonly IDbProvider provider;

        public StaticDao(IDbProvider provider)
        {
            this.provider = provider;
        }

        public int Execute()
        {
            using (var con = provider.CreateConnection())
            using (var cmd = con.CreateCommand())
            {
                var param1 = cmd.CreateParameter();
                cmd.Parameters.Add(param1);
                var param2 = cmd.CreateParameter();
                cmd.Parameters.Add(param2);

                return cmd.ExecuteNonQuery();
            }
        }
    }
}
