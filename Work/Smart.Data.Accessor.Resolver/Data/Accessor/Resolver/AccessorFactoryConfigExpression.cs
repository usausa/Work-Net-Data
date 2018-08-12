namespace Smart.Data.Accessor.Resolver
{
    using System;
    using Smart.Data.Accessor.Connection;
    using Smart.Data.Accessor.Executor;
    using Smart.Resolver;

    internal sealed class AccessorFactoryConfigExpression : IAccessorFactoryConfigExpression
    {
        public IExecutor Executor { get; private set; }

        public Type ExecutorType { get; private set; }

        public Func<IKernel, IExecutor> ExecutorFactory { get; set; }

        public IConnectionManager ConnectionManager { get; private set; }

        public Type ConnectionManagerType { get; private set; }

        public Func<IKernel, IConnectionManager> ConnectionManagerFactory { get; set; }

        public IAccessorFactoryConfigExpression UseExecutor<T>()
            where T : IExecutor
        {
            Executor = null;
            ExecutorType = typeof(T);
            ExecutorFactory = null;
            return this;
        }

        public IAccessorFactoryConfigExpression UseExecutor(IExecutor executor)
        {
            Executor = executor;
            ExecutorType = null;
            ExecutorFactory = null;
            return this;
        }

        public IAccessorFactoryConfigExpression UseExecutor(Func<IKernel, IExecutor> facory)
        {
            Executor = null;
            ExecutorType = null;
            ExecutorFactory = facory;
            return this;
        }

        public IAccessorFactoryConfigExpression UseConnectionManager<T>()
            where T : IConnectionManager
        {
            ConnectionManager = null;
            ConnectionManagerType = typeof(T);
            ConnectionManagerFactory = null;
            return this;
        }

        public IAccessorFactoryConfigExpression UseConnectionManager(IConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
            ConnectionManagerType = null;
            ConnectionManagerFactory = null;
            return this;
        }

        public IAccessorFactoryConfigExpression UseConnectionManager(Func<IKernel, IConnectionManager> facory)
        {
            ConnectionManager = null;
            ConnectionManagerType = null;
            ConnectionManagerFactory = facory;
            return this;
        }
    }
}
