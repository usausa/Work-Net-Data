namespace WorkResolver.Library
{
    using System;

    public class AccessorFactory
    {
        private readonly IExecutor executor;

        private readonly IConnectionManager connectionManager;

        public AccessorFactory(IExecutor executor, IConnectionManager connectionManager)
        {
            this.executor = executor;
            this.connectionManager = connectionManager;
        }

        public T Create<T>()
        {
            var ifType = typeof(T);
            var implName = ifType.Namespace + "." + ifType.Name.Substring(1) + "Impl";
            var implType = ifType.Assembly.GetType(implName);
            return (T)Activator.CreateInstance(implType, executor, connectionManager);
        }
    }
}
