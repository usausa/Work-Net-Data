﻿namespace WorkResolver.Usage.SmartResolverUsage
{
    using System;

    using Smart.Resolver;

    using WorkResolver.Library;

    public interface IAccessorFactoryConfigExpression
    {
        IAccessorFactoryConfigExpression UseExecutor<T>() where T : IExecutor;

        IAccessorFactoryConfigExpression UseExecutor(IExecutor executor);

        IAccessorFactoryConfigExpression UseExecutor(Func<IKernel, IExecutor> facory);

        IAccessorFactoryConfigExpression UseConnectionManager<T>() where T : IConnectionManager;

        IAccessorFactoryConfigExpression UseConnectionManager(IConnectionManager connectionManager);

        IAccessorFactoryConfigExpression UseConnectionManager(Func<IKernel, IConnectionManager> facory);
    }
}
