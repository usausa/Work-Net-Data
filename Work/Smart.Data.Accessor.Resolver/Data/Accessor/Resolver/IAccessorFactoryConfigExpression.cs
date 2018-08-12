namespace Smart.Data.Accessor.Resolver
{
    using System;

    using Smart.Data.Accessor.Connection;
    using Smart.Data.Accessor.Executor;
    using Smart.Resolver;

    public interface IAccessorFactoryConfigExpression
    {
        IAccessorFactoryConfigExpression UseExecutor<T>()
            where T : IExecutor;

        IAccessorFactoryConfigExpression UseExecutor(IExecutor executor);

        IAccessorFactoryConfigExpression UseExecutor(Func<IKernel, IExecutor> facory);

        IAccessorFactoryConfigExpression UseConnectionManager<T>()
            where T : IConnectionManager;

        IAccessorFactoryConfigExpression UseConnectionManager(IConnectionManager connectionManager);

        IAccessorFactoryConfigExpression UseConnectionManager(Func<IKernel, IConnectionManager> facory);
    }
}
