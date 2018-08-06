namespace Smart.Data.Accessor
{
    using Smart.Data.Accessor.Connection;
    using Smart.Data.Accessor.Executor;

    public sealed class AccessorFactoryConfig : IAccessorFactoryConfig
    {
        public IExecutor Executor { get; private set; }

        public IConnectionManager ConnectionManager { get; private set; }

        public AccessorFactoryConfig()
        {
        }

        public AccessorFactoryConfig(IExecutor executor, IConnectionManager connectionManager)
        {
            Executor = executor;
            ConnectionManager = connectionManager;
        }

        public AccessorFactoryConfig UseExecutor(IExecutor executor)
        {
            Executor = executor;
            return this;
        }

        public AccessorFactoryConfig UseConnectionManager(IConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
            return this;
        }
    }
}
