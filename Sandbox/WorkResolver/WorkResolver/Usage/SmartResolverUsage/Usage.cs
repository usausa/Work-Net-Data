namespace WorkResolver.Usage.SmartResolverUsage
{
    using System;
    using System.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Data.Sqlite;

    using Smart.ComponentModel;
    using Smart.Resolver;
    using Smart.Resolver.Bindings;
    using Smart.Resolver.Handlers;
    using Smart.Resolver.Providers;

    using WorkResolver.Accessor;
    using WorkResolver.External;
    using WorkResolver.Library;

    public class Usage
    {
        public static void UseSimple()
        {
            var config = new ResolverConfig();

            config
                .Bind<IConnectionFactory>()
                .ToConstant(new CallbackConnectionFactory(() => new SqliteConnection("Data Source=:memory:")))
                .InSingletonScope();

            config.AddAccessorFactory(c =>
            {
                c.UseExecutor<ExecutorImpl>();
                c.UseConnectionManager<ConnectionFactoryConnectionManager>();
            });

            config.Bind<Service>().ToSelf().InSingletonScope();

            var resolver = config.ToResolver();

            var service = resolver.Get<Service>();
            service.Action();
        }

        public static void UseMultiple()
        {
            var config = new ResolverConfig();

            config
                .Bind<IConnectionFactory>()
                .ToConstant(new CallbackConnectionFactory(() => new SqliteConnection("Data Source=:memory:")))
                .InSingletonScope()
                .Named(string.Empty);
            config
                .Bind<IConnectionFactory>()
                .ToConstant(new CallbackConnectionFactory(() => new SqliteConnection("Data Source=:memory:")))
                .InSingletonScope()
                .Named("Sub");

            config.AddAccessorFactory(c =>
            {
                c.UseExecutor<ExecutorImpl>();
                c.UseConnectionManager(k => new MultipleResolverConnectionManager(k));
            });

            config.Bind<Service>().ToSelf().InSingletonScope();

            var resolver = config.ToResolver();

            var service = resolver.Get<Service>();
            service.Action();
        }

        public class Service
        {
            private readonly IHogeDao hogeDao;

            public Service(IHogeDao hogeDao)
            {
                this.hogeDao = hogeDao;
            }

            public void Action()
            {
                hogeDao.Execute();
            }
        }
    }

    public sealed class MultipleResolverConnectionManager : IConnectionManager
    {
        private readonly IKernel kernel;

        public MultipleResolverConnectionManager(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public Func<IDbConnection> GetFactory(string name)
        {
            return kernel.Get<IConnectionFactory>(name).CreateConnection;
        }
    }

    public interface IAccessorFactoryConfigExpression
    {
        IAccessorFactoryConfigExpression UseExecutor<T>() where T : IExecutor;

        IAccessorFactoryConfigExpression UseExecutor(IExecutor executor);

        IAccessorFactoryConfigExpression UseExecutor(Func<IKernel, IExecutor> facory);

        IAccessorFactoryConfigExpression UseConnectionManager<T>() where T : IConnectionManager;

        IAccessorFactoryConfigExpression UseConnectionManager(IConnectionManager connectionManager);

        IAccessorFactoryConfigExpression UseConnectionManager(Func<IKernel, IConnectionManager> facory);
    }

    internal sealed class AccessorFactoryConfigExpression : IAccessorFactoryConfigExpression
    {
        public IExecutor Executor { get; private set; }

        public Type ExecutorType { get; private set; }

        public Func<IKernel, IExecutor> ExecutorFactory { get; set; }

        public IConnectionManager ConnectionManager { get; private set; }

        public Type ConnectionManagerType { get; private set; }

        public Func<IKernel, IConnectionManager> ConnectionManagerFactory { get; set; }

        public IAccessorFactoryConfigExpression UseExecutor<T>() where T : IExecutor
        {
            Executor = null;
            ExecutorType = typeof(T);
            ExecutorFactory = null;
            return this;
        }

        public IAccessorFactoryConfigExpression UseExecutor(IExecutor executor)
        {
            Executor = executor;
            ExecutorType = null;
            ExecutorFactory = null;
            return this;
        }

        public IAccessorFactoryConfigExpression UseExecutor(Func<IKernel, IExecutor> facory)
        {
            Executor = null;
            ExecutorType = null;
            ExecutorFactory = facory;
            return this;
        }

        public IAccessorFactoryConfigExpression UseConnectionManager<T>() where T : IConnectionManager
        {
            ConnectionManager = null;
            ConnectionManagerType = typeof(T);
            ConnectionManagerFactory = null;
            return this;
        }

        public IAccessorFactoryConfigExpression UseConnectionManager(IConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
            ConnectionManagerType = null;
            ConnectionManagerFactory = null;
            return this;
        }

        public IAccessorFactoryConfigExpression UseConnectionManager(Func<IKernel, IConnectionManager> facory)
        {
            ConnectionManager = null;
            ConnectionManagerType = null;
            ConnectionManagerFactory = facory;
            return this;
        }
    }

    public sealed class DaoMissingHandler : IMissingHandler
    {
        public IEnumerable<IBinding> Handle(IComponentContainer components, IBindingTable table, Type type)
        {
            if (type.GetCustomAttribute<DaoAttribute>() != null)
            {
                return new[]
                {
                    new Binding(type, new CallbackProvider(type, kernel => kernel.Get<IAccessorFactory>().Create(type)))
                };
            }

            return Enumerable.Empty<IBinding>();
        }
    }

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

            config.UseMissingHandler<DaoMissingHandler>();

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
