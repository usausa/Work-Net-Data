namespace Smart.Data.Accessor.Resolver
{
    using System;

    using Smart.Data.Accessor.Connection;
    using Smart.Data.Accessor.Executor;
    using Smart.Resolver;

    public static class ResolverConfigExtensions
    {
        public static ResolverConfig AddAccessorFactory(this ResolverConfig config)
        {
            return config.AddAccessorFactory(c => { });
        }

        public static ResolverConfig AddAccessorFactory(this ResolverConfig config, Action<IAccessorFactoryConfigExpression> action)
        {
            var expression = new AccessorFactoryConfigExpression();
            action(expression);

            config.UseMissingHandler<AccessorMissingHandler>();

            if (expression.Executor != null)
            {
                config.Bind<IExecutor>().ToConstant(expression.Executor).InSingletonScope();
            }
            else if (expression.ExecutorType != null)
            {
                config.Bind<IExecutor>().To(expression.ExecutorType).InSingletonScope();
            }
            else if (expression.ExecutorFactory != null)
            {
                config.Bind<IExecutor>().ToMethod(expression.ExecutorFactory).InSingletonScope();
            }

            if (expression.ConnectionManager != null)
            {
                config.Bind<IConnectionManager>().ToConstant(expression.ConnectionManager).InSingletonScope();
            }
            else if (expression.ConnectionManagerType != null)
            {
                config.Bind<IConnectionManager>().To(expression.ConnectionManagerType).InSingletonScope();
            }
            else if (expression.ConnectionManagerFactory != null)
            {
                config.Bind<IConnectionManager>().ToMethod(expression.ConnectionManagerFactory).InSingletonScope();
            }

            config.Bind<IAccessorFactoryConfig>().To<AccessorFactoryConfig>();
            config.Bind<IAccessorFactory>().To<AccessorFactory>();

            return config;
        }
    }
}
