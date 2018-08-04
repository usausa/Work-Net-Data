namespace WorkResolver.Usage.ServiceCollectionUsage
{
    using System;

    using WorkResolver.Library;

    public interface IAccessorFactoryConfigExpression
    {
        IAccessorFactoryConfigExpression UseExecutor<T>() where T : IExecutor;

        IAccessorFactoryConfigExpression UseExecutor(IExecutor executor);

        IAccessorFactoryConfigExpression UseExecutor(Func<IServiceProvider, IExecutor> facory);

        IAccessorFactoryConfigExpression UseConnectionManager<T>() where T : IConnectionManager;

        IAccessorFactoryConfigExpression UseConnectionManager(IConnectionManager connectionManager);

        IAccessorFactoryConfigExpression UseConnectionManager(Func<IServiceProvider, IConnectionManager> facory);
    }
}
