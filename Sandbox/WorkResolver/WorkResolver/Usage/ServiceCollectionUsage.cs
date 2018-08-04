using System.Collections.Generic;
using System.Data;

namespace WorkResolver.Usage
{
    using System;

    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.DependencyInjection;

    using WorkResolver.Accessor;
    using WorkResolver.External;
    using WorkResolver.Library;

    public class ServiceCollectionUsage
    {
        public static void UseSimple()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IConnectionFactory>(p =>
                new CallbackConnectionFactory(() => new SqliteConnection("Data Source=:memory:")));

            services.AddAccessorFactory(config =>
            {
                config.UseExecutor<ExecutorImpl>();
                config.UseConnectionManager<ConnectionFactoryConnectionManager>();
            });

            services.AddSingleton<Service>();

            var provider = services.BuildServiceProvider();

            var service = provider.GetService<Service>();
            service.Action();
        }

        public static void UseMultiple()
        {
            var services = new ServiceCollection();

            services.AddAccessorFactory(config =>
            {
                config.UseExecutor<ExecutorImpl>();
                config.UseConnectionManager(new MultipleConnectionManager(new Dictionary<string, Func<IDbConnection>>
                {
                    { "", () => new SqliteConnection("Data Source=:memory:") }
                }));
            });

            services.AddSingleton<Service>();

            var provider = services.BuildServiceProvider();

            var service = provider.GetService<Service>();
            service.Action();
        }
    }

    public interface IAccessorConfigExpression
    {
        IAccessorConfigExpression UseExecutor<T>() where T : IExecutor;

        IAccessorConfigExpression UseExecutor(IExecutor executor);

        IAccessorConfigExpression UseExecutor(Func<IServiceProvider, IExecutor> facory);

        IAccessorConfigExpression UseConnectionManager<T>() where T : IConnectionManager;

        IAccessorConfigExpression UseConnectionManager(IConnectionManager connectionManager);

        IAccessorConfigExpression UseConnectionManager(Func<IServiceProvider, IConnectionManager> facory);
    }

    internal sealed class AccessorConfigExpression : IAccessorConfigExpression
    {
        public IExecutor Executor { get; private set; }

        public Type ExecutorType { get; private set; }

        public Func<IServiceProvider, IExecutor> ExecutorFactory { get; set; }

        public IConnectionManager ConnectionManager { get; private set; }

        public Type ConnectionManagerType { get; private set; }

        public Func<IServiceProvider, IConnectionManager> ConnectionManagerFactory { get; set; }

        public IAccessorConfigExpression UseExecutor<T>() where T : IExecutor
        {
            Executor = null;
            ExecutorType = typeof(T);
            ExecutorFactory = null;
            return this;
        }

        public IAccessorConfigExpression UseExecutor(IExecutor executor)
        {
            Executor = executor;
            ExecutorType = null;
            ExecutorFactory = null;
            return this;
        }

        public IAccessorConfigExpression UseExecutor(Func<IServiceProvider, IExecutor> facory)
        {
            Executor = null;
            ExecutorType = null;
            ExecutorFactory = facory;
            return this;
        }

        public IAccessorConfigExpression UseConnectionManager<T>() where T : IConnectionManager
        {
            ConnectionManager = null;
            ConnectionManagerType = typeof(T);
            ConnectionManagerFactory = null;
            return this;
        }

        public IAccessorConfigExpression UseConnectionManager(IConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
            ConnectionManagerType = null;
            ConnectionManagerFactory = null;
            return this;
        }

        public IAccessorConfigExpression UseConnectionManager(Func<IServiceProvider, IConnectionManager> facory)
        {
            ConnectionManager = null;
            ConnectionManagerType = null;
            ConnectionManagerFactory = facory;
            return this;
        }
    }

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

    public class Service
    {
        private readonly IHogeDao hogeDao;

        public Service(IAccessorFactory accessorFactory)
        {
            hogeDao = accessorFactory.Create<IHogeDao>();
        }

        public void Action()
        {
            hogeDao.Execute();
        }
    }
}
