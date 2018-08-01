namespace WorkResolver.Library
{
    using System;

    public class DaoFactory
    {
        private readonly IExecutor executor;

        private readonly IConnectionManager connectionManager;

        public DaoFactory(IExecutor executor, IConnectionManager connectionManager)
        {
            this.executor = executor;
            this.connectionManager = connectionManager;
        }

        public T Create<T>()
        {
            return (T)Activator.CreateInstance(typeof(T), executor, connectionManager);
        }
    }
}
