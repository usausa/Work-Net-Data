namespace WorkResolver.Usage.ServiceCollectionUsage
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.DependencyInjection;

    using WorkResolver.Accessor;
    using WorkResolver.External;
    using WorkResolver.Library;

    public class Usage
    {
        public static void UseSimple()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IConnectionFactory>(p =>
                new CallbackConnectionFactory(() => new SqliteConnection("Data Source=:memory:")));

            services.AddAccessorFactory(c =>
            {
                c.UseExecutor<ExecutorImpl>();
                c.UseConnectionManager<ConnectionFactoryConnectionManager>();
            });

            services.AddSingleton<Service>();

            var provider = services.BuildServiceProvider();

            var service = provider.GetService<Service>();
            service.Action();
        }

        public static void UseMultiple()
        {
            var services = new ServiceCollection();

            services.AddAccessorFactory(c =>
            {
                c.UseExecutor<ExecutorImpl>();
                c.UseConnectionManager(new MultipleConnectionManager(new Dictionary<string, Func<IDbConnection>>
                {
                    { string.Empty, () => new SqliteConnection("Data Source=:memory:") }
                }));
            });

            services.AddSingleton<Service>();

            var provider = services.BuildServiceProvider();

            var service = provider.GetService<Service>();
            service.Action();
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
}
