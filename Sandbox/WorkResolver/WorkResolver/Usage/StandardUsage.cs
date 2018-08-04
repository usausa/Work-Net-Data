namespace WorkResolver.Usage
{
    using Microsoft.Data.Sqlite;

    using WorkResolver.Accessor;
    using WorkResolver.Library;

    public static class StandardUsage
    {
        public static void Use()
        {
            var factory = new AccessorFactory(
                new ExecutorImpl(),
                new SingleConnectionManager(() => new SqliteConnection("Data Source=:memory:")));

            var hogeDao = factory.Create<IHogeDao>();
            hogeDao.Execute();
        }
    }
}
