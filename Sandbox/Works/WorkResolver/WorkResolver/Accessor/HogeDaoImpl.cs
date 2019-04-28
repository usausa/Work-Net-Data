namespace WorkResolver.Accessor
{
    using System;
    using System.Data;

    using WorkResolver.Library;

    public sealed class HogeDaoImpl : IHogeDao
    {
        private readonly IExecutor executor;

        private readonly Func<IDbConnection> connectionFactory;

        public HogeDaoImpl(IExecutor executor, IConnectionManager connectionManager)
        {
            this.executor = executor;
            connectionFactory = connectionManager.GetFactory(string.Empty);
        }

        public void Execute()
        {
            executor.Execute(connectionFactory());
        }
    }
}
