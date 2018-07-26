﻿namespace WorkResolver.Accessor
{
    using System;

    using WorkResolver.Library;

    public class HogeDaoImpl : IHogeDao
    {
        private readonly IExecutor executor;

        private readonly Func<IConnection> connectionFactory;

        public HogeDaoImpl(IExecutor executor, IConnectionManager connectionManager)
        {
            this.executor = executor;
            connectionFactory = connectionManager.GetFactory(string.Empty);
        }

        public void Execute()
        {
            executor.Execute(connectionFactory());
        }
    }
}
