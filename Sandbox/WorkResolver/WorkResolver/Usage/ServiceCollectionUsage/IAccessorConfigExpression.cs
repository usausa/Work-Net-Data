namespace WorkResolver.Usage.ServiceCollectionUsage
{
    using System;

    using WorkResolver.Library;

    public interface IAccessorConfigExpression
    {
        IAccessorConfigExpression UseExecutor<T>() where T : IExecutor;

        IAccessorConfigExpression UseExecutor(IExecutor executor);

        IAccessorConfigExpression UseExecutor(Func<IServiceProvider, IExecutor> facory);

        IAccessorConfigExpression UseConnectionManager<T>() where T : IConnectionManager;

        IAccessorConfigExpression UseConnectionManager(IConnectionManager connectionManager);

        IAccessorConfigExpression UseConnectionManager(Func<IServiceProvider, IConnectionManager> facory);
    }
}
