namespace Smart.Data.Accessor.DependencyInjection
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    using Smart.Data.Accessor.Connection;
    using Smart.Data.Accessor.Executor;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAccessorFactory(this IServiceCollection services)
        {
            return services.AddAccessorFactory(config => { });
        }

        public static IServiceCollection AddAccessorFactory(this IServiceCollection services, Action<IAccessorFactoryConfigExpression> action)
        {
            var expression = new AccessorFactoryConfigExpression();
            action(expression);

            if (expression.Executor != null)
            {
                services.AddSingleton(expression.Executor);
            }
            else if (expression.ExecutorType != null)
            {
                services.AddSingleton(typeof(IExecutor), expression.ExecutorType);
            }
            else if (expression.ExecutorFactory != null)
            {
                services.AddSingleton(expression.ExecutorFactory);
            }

            if (expression.ConnectionManager != null)
            {
                services.AddSingleton(expression.ConnectionManager);
            }
            else if (expression.ConnectionManagerType != null)
            {
                services.AddSingleton(typeof(IConnectionManager), expression.ConnectionManagerType);
            }
            else if (expression.ConnectionManagerFactory != null)
            {
                services.AddSingleton(expression.ConnectionManagerFactory);
            }

            services.AddSingleton<IAccessorFactoryConfig, AccessorFactoryConfig>();
            services.AddSingleton<IAccessorFactory, AccessorFactory>();

            return services;
        }
    }
}
