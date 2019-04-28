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
                c.UseConnectionManager(k => new NamedResolverConnectionManager(k));
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
}
