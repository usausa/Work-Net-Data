namespace WorkResolver.Usage
{
    using Microsoft.Data.Sqlite;

    using WorkResolver.Accessor;
    using WorkResolver.Library;

    public static class StandardUsage
    {
        public static void Use()
        {
            var factory = new AccessorFactoryConfig()
                .UseExecutor(new ExecutorImpl())
                .UseConnectionManager(new SingleConnectionManager(() => new SqliteConnection("Data Source=:memory:")))
                .ToAccessorFactory();

            var hogeDao = factory.Create<IHogeDao>();
            hogeDao.Execute();
        }
    }
}
