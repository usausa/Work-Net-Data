namespace WorkResolver.Usage
{
    using WorkResolver.Accessor;
    using WorkResolver.Library;

    public static class StandardUsage
    {
        public static void Use()
        {
            // TODO use executor
            // TODO use connection
            var factory = new DaoFactory(
                new ExecutorImpl(),
                new SingleConnectionManager(() => null));

            var hogeDao = factory.Create<IHogeDao>();
            hogeDao.Execute();
        }
    }
}
