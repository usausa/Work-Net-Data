namespace WorkResolver.Usage.ServiceCollectionUsage
{
    using System;

    using WorkResolver.Library;

    internal sealed class AccessorConfigExpression : IAccessorConfigExpression
    {
        public IExecutor Executor { get; private set; }

        public Type ExecutorType { get; private set; }

        public Func<IServiceProvider, IExecutor> ExecutorFactory { get; set; }

        public IConnectionManager ConnectionManager { get; private set; }

        public Type ConnectionManagerType { get; private set; }

        public Func<IServiceProvider, IConnectionManager> ConnectionManagerFactory { get; set; }

        public IAccessorConfigExpression UseExecutor<T>() where T : IExecutor
        {
            Executor = null;
            ExecutorType = typeof(T);
            ExecutorFactory = null;
            return this;
        }

        public IAccessorConfigExpression UseExecutor(IExecutor executor)
        {
            Executor = executor;
            ExecutorType = null;
            ExecutorFactory = null;
            return this;
        }

        public IAccessorConfigExpression UseExecutor(Func<IServiceProvider, IExecutor> facory)
        {
            Executor = null;
            ExecutorType = null;
            ExecutorFactory = facory;
            return this;
        }

        public IAccessorConfigExpression UseConnectionManager<T>() where T : IConnectionManager
        {
            ConnectionManager = null;
            ConnectionManagerType = typeof(T);
            ConnectionManagerFactory = null;
            return this;
        }

        public IAccessorConfigExpression UseConnectionManager(IConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
            ConnectionManagerType = null;
            ConnectionManagerFactory = null;
            return this;
        }

        public IAccessorConfigExpression UseConnectionManager(Func<IServiceProvider, IConnectionManager> facory)
        {
            ConnectionManager = null;
            ConnectionManagerType = null;
            ConnectionManagerFactory = facory;
            return this;
        }
    }
}
