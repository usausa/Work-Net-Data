namespace WorkResolver.Usage.ServiceCollectionUsage
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    using WorkResolver.Library;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAccessorFactory(this IServiceCollection services)
        {
            return services.AddAccessorFactory(config => { });
        }

        public static IServiceCollection AddAccessorFactory(this IServiceCollection services, Action<IAccessorConfigExpression> config)
        {
            var expression = new AccessorConfigExpression();
            config(expression);

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
